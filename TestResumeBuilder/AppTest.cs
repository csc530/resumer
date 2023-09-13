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
		public void Test1()
		{
			var args = new string[] { };
			cliapp.Run(new[] { "add", "" });

			Assert.That(console.Output, Does.Contain("err"));
		}
	}
}