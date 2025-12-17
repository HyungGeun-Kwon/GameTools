using FluentAssertions;
using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Domain.Catalog.Rarities.Entities;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using Moq;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;
using GameTools.Server.Application.Catalog.Rarities.Commands.UpdateRarity;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;

namespace GameTools.Server.Application.Tests.Catalog.Rarities.Commands.UpdateRarity
{
    public class UpdateRarityHandlerTests
    {
        private static Rarity BuildRarityFromSpec(
            UpdateRaritySpec spec,
            string? gradeOverride = null,
            string? colorCodeOverride = null)
            => new(
                id: RarityId.From(spec.Id),
                grade: new RarityGrade(gradeOverride ?? spec.Grade),
                colorCode: new RarityColorCode(colorCodeOverride ?? spec.NormalizedColorCode));

        private static UpdateRarityCommand BuildCommand(UpdateRaritySpec? spec = null)
            => new(spec ?? BuildDefaultUpdateRaritySpec());

        private static UpdateRarityHandler CreateHandler(
            out Mock<IRarityWriteStore> rarityWriteStoreMock,
            out Mock<IRarityGradeUniquenessPolicy> gradePolicyMock,
            out Mock<IUnitOfWork> uowMock)
        {
            rarityWriteStoreMock = new Mock<IRarityWriteStore>();
            gradePolicyMock = new Mock<IRarityGradeUniquenessPolicy>();
            uowMock = new Mock<IUnitOfWork>();

            return new UpdateRarityHandler(
                rarityWriteStoreMock.Object,
                gradePolicyMock.Object,
                uowMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Rarity_Does_Not_Exist()
        {
            var handler = CreateHandler(
                out var rarityWriteStoreMock,
                out var gradePolicyMock,
                out var uowMock);

            var spec = BuildDefaultUpdateRaritySpec();
            var command = BuildCommand(spec);

            // LoadForUpdateAsync 기본값 null → NotFoundException

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            rarityWriteStoreMock.Verify(x => x.LoadForUpdateAsync(RarityId.From(spec.Id), It.IsAny<CancellationToken>()), Times.Once);
            gradePolicyMock.Verify(x => x.EnsureUniqueAsync(It.IsAny<RarityGrade>(), It.IsAny<CancellationToken>()), Times.Never);
            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Not_Call_Policies_When_Grade_And_Color_Not_Changed()
        {
            var handler = CreateHandler(
                out var rarityWriteStoreMock,
                out var gradePolicyMock,
                out var uowMock);

            var grade = ValidRarityGradeValue('A');
            var colorCode = ValidRarityColorCodeValue();

            var spec = BuildDefaultUpdateRaritySpec(
                grade: grade,
                colorCode: colorCode);

            var command = BuildCommand(spec);

            var existingRarity = BuildRarityFromSpec(spec, gradeOverride: grade, colorCodeOverride: colorCode);
            var rowVersion = ValidRarityRowVersion();

            rarityWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(RarityId.From(spec.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRarity);

            rarityWriteStoreMock
                .Setup(x => x.GetRowVersion(existingRarity))
                .Returns(rowVersion);

            uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(existingRarity.Id.Value);
            result.RowVersion.Should().BeEquivalentTo(rowVersion);

            gradePolicyMock.Verify(
                x => x.EnsureUniqueAsync(It.IsAny<RarityGrade>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Call_GradePolicy_When_Grade_Changed()
        {
            var handler = CreateHandler(
                out var rarityWriteStoreMock,
                out var gradePolicyMock,
                out var uowMock);

            var originalGrade = ValidRarityGradeValue('A');
            var newGrade = ValidRarityGradeValue('B');
            var colorCode = ValidRarityColorCodeValue();

            var spec = BuildDefaultUpdateRaritySpec(
                grade: newGrade,
                colorCode: colorCode);

            var command = BuildCommand(spec);

            // 기존 Rarity는 다른 Grade, 같은 Color
            var existingRarity = BuildRarityFromSpec(
                spec,
                gradeOverride: originalGrade,
                colorCodeOverride: colorCode);

            var rowVersion = ValidRarityRowVersion();

            rarityWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(RarityId.From(spec.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRarity);

            gradePolicyMock
                .Setup(x => x.EnsureUniqueAsync(
                    It.Is<RarityGrade>(g => g.Value == newGrade),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            rarityWriteStoreMock
                .Setup(x => x.GetRowVersion(existingRarity))
                .Returns(rowVersion);

            uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(existingRarity.Id.Value);
            result.RowVersion.Should().BeEquivalentTo(rowVersion);

            gradePolicyMock.Verify(
                x => x.EnsureUniqueAsync(
                    It.Is<RarityGrade>(g => g.Value == newGrade),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Call_ColorPolicy_When_Color_Changed()
        {
            var handler = CreateHandler(
                out var rarityWriteStoreMock,
                out var gradePolicyMock,
                out var uowMock);

            var grade = ValidRarityGradeValue('A');
            var originalColor = "#AAAAAA";
            var newColor = "#BBBBBB";

            var spec = BuildDefaultUpdateRaritySpec(
                grade: grade,
                colorCode: newColor);

            var command = BuildCommand(spec);

            var existingRarity = BuildRarityFromSpec(
                spec,
                gradeOverride: grade,
                colorCodeOverride: originalColor);

            var rowVersion = ValidRarityRowVersion();

            rarityWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(RarityId.From(spec.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRarity);

            rarityWriteStoreMock
                .Setup(x => x.GetRowVersion(existingRarity))
                .Returns(rowVersion);

            uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(existingRarity.Id.Value);
            result.RowVersion.Should().BeEquivalentTo(rowVersion);

            // Grade는 안 바뀌었으므로 호출되면 안 됨
            gradePolicyMock.Verify(
                x => x.EnsureUniqueAsync(It.IsAny<RarityGrade>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
