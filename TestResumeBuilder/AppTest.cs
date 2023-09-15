using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			Assert.Catch(() => cliapp.Run(new[] { "add", "job" }), "Expected to fail");
		}

		[Test]
		public void AddJob_WithoutStartDate_ShouldFail([Random(100, Distinct = true)] string title)
		{
			var args = new string[] { "add", "job", "--title", title };
			int exitcode = cliapp.Run(args);
			Assume.That(title.Length < 100 && title.Length > 0);
			Assert.Multiple(() =>
			{
				Assert.That(exitcode, Is.EqualTo(0));
				Assert.That(console.Output, Contains.Value(title));
			});
		}
	}
}