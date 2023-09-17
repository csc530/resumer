using resume_builder.cli;
using resume_builder.cli.commands;
using Spectre.Console.Testing;

namespace TestResumeBuilder
{
	public class AppTest
	{
		internal Spectre.Console.Testing.CommandAppTester TestApp;

		[SetUp]
		public void Setup()
		{
			TestApp = new();
			TestApp.Configure(Program.appConfiguration);
		}
	}
}