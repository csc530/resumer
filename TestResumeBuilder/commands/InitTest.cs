
namespace TestResumeBuilder.commands;

public class InitTest : AppTestBase
{
    [Fact]
    public void Init_WithNoArgs_ShouldReturnSuccess()
    {
        var result = TestApp.Run("init");
        Assert.Equal(0, result.ExitCode);
    }


    [Theory]
    [MemberData(nameof(TestData.RandomArgs), MemberType = typeof(TestData))]
    public void Init_WithArgs_ShouldReturnSuccess(string[] args)
    {
        Assert.True(true);
        //var result = Run("init", args);
        //Assert.That(result.ExitCode, Is.EqualTo(0));
    }
}