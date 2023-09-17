using Spectre.Console.Testing;

namespace TestResumeBuilder
{
	public class AppTest
	{
		internal resume_builder.App cliapp;
		internal TestConsole console;

		[SetUp]
		public void Setup()
		{
			console = new TestConsole();
			cliapp = new resume_builder.App(console);
		}
	}
}