using FluentAssertions;
using FluentValidation;
using GameTools.Server.Application.Behaviors;
using AppValidationException = GameTools.Server.Application.Abstractions.Exceptions.ValidationException;

namespace GameTools.Server.Application.Tests.Behaviors
{
    public class ValidationBehaviorTests
    {
        private sealed record TestRequest(string Value);

        private sealed record TestResponse(string Value);

        private sealed class TestRequestValidator : AbstractValidator<TestRequest>
        {
            public TestRequestValidator()
            {
                RuleFor(x => x.Value).NotEmpty();
            }
        }

        [Fact]
        public async Task Handle_Should_Call_Next_When_No_Validators()
        {
            var validators = Array.Empty<IValidator<TestRequest>>();
            var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

            var request = new TestRequest("ok");
            var expected = new TestResponse("from-next");
            var called = false;

            Task<TestResponse> next(CancellationToken _ = default)
            {
                called = true;
                return Task.FromResult(expected);
            }

            var result = await behavior.Handle(request, next, CancellationToken.None);

            called.Should().BeTrue();
            result.Should().Be(expected);
        }

        [Fact]
        public async Task Handle_Should_Call_Next_When_Validation_Passes()
        {
            var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
            var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

            var request = new TestRequest("valid");
            var expected = new TestResponse("ok");
            var called = false;

            Task<TestResponse> next(CancellationToken _ = default)
            {
                called = true;
                return Task.FromResult(expected);
            }

            var result = await behavior.Handle(request, next, CancellationToken.None);

            called.Should().BeTrue();
            result.Should().Be(expected);
        }

        [Fact]
        public async Task Handle_Should_Throw_ValidationException_When_Validation_Fails()
        {
            var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
            var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

            var request = new TestRequest("");
            var called = false;

            Task<TestResponse> next(CancellationToken _ = default)
            {
                called = true;
                return Task.FromResult(new TestResponse("should-not-be-called"));
            }

            Func<Task> act = async () =>
                await behavior.Handle(request, next, CancellationToken.None);

            await act.Should().ThrowAsync<AppValidationException>();
            called.Should().BeFalse();
        }
    }
}
