using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;
using GameTools.Server.Application.Catalog.Rarities.Models;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;

namespace GameTools.Server.TestUtilities.Application.Rarities
{
    public class AppRarityTestData
    {
        public static byte[] ValidRarityRowVersion()
            => Convert.FromBase64String("AAAAAAAAAAA=");

        public static RarityReadModel BuildDefaultRarityReadModel(
            Guid? id = null,
            string? grade = null,
            string? colorCode = null,
            byte[]? rowVersion = null)
            => new (
                Id: id ?? Guid.NewGuid(),
                Grade: grade ?? ValidRarityGradeValue(),
                ColorCode: colorCode ?? ValidRarityColorCodeValue(),
                RowVersion: rowVersion ?? ValidRarityRowVersion());

        public static CreateRaritySpec BuildDefaultCreateRaritySpec(
            string? grade = null,
            string? colorCode = null)
            => new (
                Grade: grade ?? ValidRarityGradeValue(),
                ColorCode: colorCode ?? ValidRarityColorCodeValue());

        public static UpdateRaritySpec BuildDefaultUpdateRaritySpec(
            Guid? id = null,
            string? grade = null,
            string? colorCode = null,
            byte[]? rowVersion = null)
            => new (
                Id: id ?? Guid.NewGuid(),
                Grade: grade ?? ValidRarityGradeValue(),
                ColorCode: colorCode ?? ValidRarityColorCodeValue(),
                RowVersion: rowVersion ?? ValidRarityRowVersion());

        public static DeleteRaritySpec BuildDefaultDeleteRaritySpec(
            Guid? id = null,
            byte[]? rowVersion = null)
            => new (
                Id: id ?? Guid.NewGuid(),
                RowVersion: rowVersion ?? ValidRarityRowVersion());
    }
}
