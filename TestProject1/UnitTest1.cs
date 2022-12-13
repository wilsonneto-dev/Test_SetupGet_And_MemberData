using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace TestProject1;

public interface IPayload
{
    string Name { get; }
    string LastName { get; }
}

public class UnitTest1
{
    [Theory,
        MemberData(memberName: nameof(GetProperties))]
    public void Test1(Func<IPayload, string> lambda,string value, string expected)
    {
        var p = Mock.Of<IPayload>();
        Mock.Get(p).SetupGet(lambda).Returns(value);
        p.Name.Should().Be(expected);
    }

    public static IEnumerable<object[]> GetProperties() => new List<object[]>(){
        new object[] { (IPayload x) => x.Name, "teste", "teste" }
    };
}
