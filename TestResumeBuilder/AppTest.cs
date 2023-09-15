using Spectre.Console.Testing;

namespace TestResumeBuilder
{
	public class Tests
	{
		resume_builder.App cliapp;
		TestConsole console;

		[SetUp]
		public void Setup()
		{
			console = new TestConsole();
			cliapp = new resume_builder.App(console);
		}

		[Test]
		public void AddJob_WithNoArguments_ShouldFail()
		{
			var args = new string[] { };
			Assert.Catch(()=>cliapp.Run(new[] { "add", "job" }), "Expected to fail");
		}
	}
}