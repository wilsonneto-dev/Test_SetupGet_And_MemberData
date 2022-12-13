using FluentAssertions;
using FluentValidation;
using Moq;

namespace TestProject1;

public class UnitTest01
{
    [Theory,
        MemberData(memberName: nameof(GetProperties))]
    public void Test1(Func<Mock<IPayload>, Moq.Language.Flow.IReturnsResult<IPayload>> setup, string expectedMessage)
    {
        var payload = PayloadBuilder.Build();
        setup(Mock.Get(payload));
        var sut = new PayloadValidator();

        var validationResult = sut.Validate(payload);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.First().ErrorMessage.Should().Be(expectedMessage);
    }

    public static IEnumerable<object[]> GetProperties() => new List<object[]>(){
        new object[] {
            (Mock<IPayload> mock) => mock.SetupGet(x => x.Name).Returns(""),
            "'Name' must not be empty."
        },
        new object[] {
            (Mock<IPayload> mock) => mock.SetupGet(x => x.LastName).Returns(""),
            "'Last Name' must not be empty."
        },
        new object[] {
            (Mock<IPayload> mock) => mock.SetupGet(x => x.Number).Returns(0),
            "'Number' must be greater than '0'."
        }
    };
}

public class UnitTest02
{
    [Theory,
        MemberData(memberName: nameof(GetProperties))]
    public void Test1(IPayload payload, string expectedMessage)
    {
        var sut = new PayloadValidator();

        var validationResult = sut.Validate(payload);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.First().ErrorMessage.Should().Be(expectedMessage);
    }

    public static IEnumerable<object[]> GetProperties()
    {
        var payload = PayloadBuilder.Build();
        Mock.Get(payload).SetupGet(x => x.Name).Returns("");
        yield return new object[] { payload, "'Name' must not be empty." };

        payload = PayloadBuilder.Build();
        Mock.Get(payload).SetupGet(x => x.LastName).Returns("");
        yield return new object[] { payload, "'Last Name' must not be empty." };

        payload = PayloadBuilder.Build();
        Mock.Get(payload).SetupGet(x => x.Number).Returns(0);
        yield return new object[] { payload, "'Number' must be greater than '0'." };
    }
}
public interface IPayload
{
    string Name { get; }
    string LastName { get; }
    int Number { get; }
}

static class PayloadBuilder
{
    public static IPayload Build() => Mock.Of<IPayload>(
        x => x.Name == "Wilson" &&
        x.LastName == "Gomes" &&
        x.Number == 10);
}

class PayloadValidator : AbstractValidator<IPayload>
{
    public PayloadValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Number).GreaterThan(0);
    }
}