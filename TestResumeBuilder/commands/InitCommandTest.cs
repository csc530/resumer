using resume_builder;
using resume_builder.models;

namespace TestResumeBuilder.commands;

internal class InitCommandTest : AppTest
{
	[SetUp]
	[Test]
	public void Init_ShouldPass()
	{
		var commandAppResult = TestApp.Run("init");
		Assert.That(commandAppResult.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
	}
}