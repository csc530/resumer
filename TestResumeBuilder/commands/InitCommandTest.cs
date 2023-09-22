using resume_builder;

namespace TestResumeBuilder.commands;

internal class InitCommandTest : AppTest
{
	[SetUp]
	[Test]
	//todo:  find way to pass text to test command app for prompts
	public void Init_ShouldPass()
	{
		var commandAppResult = TestApp.Run("init");
		Assert.That(commandAppResult.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
	}
}