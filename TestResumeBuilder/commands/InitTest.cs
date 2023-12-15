namespace TestResumeBuilder.commands;

[TestFixture]
public class InitTest : AppTest
{
    [Test]
    public void Init_WithNoArgs_ShouldReturnSuccess()
    {
        var result = TestApp.Run("init");
        Assert.That(result.ExitCode, Is.EqualTo(0));
    }

    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.RandomArgs))]
    public void Init_WithArgs_ShouldReturnSuccess(string[] args)
    {
        var result = Run("init", args);
        Assert.That(result.ExitCode, Is.EqualTo(0));
    }
}