using resume_builder;
using Spectre.Console.Testing;
using System.ComponentModel;
using resume_builder.cli.commands.add;
using TestResumeBuilder.test_data;

namespace TestResumeBuilder
{
	public class AddJobTest : AppTest
	{
		[SetUp]
		public void Setup() => TestApp.Run("init");

		[Test]
		public void WithNoArguments_ShouldFail() => Assert.Catch(() => TestApp.Run("add", "job"));


		[Test]
		[TestCaseSource(typeof(TestData), nameof(TestData.Jobtitles))]
		[TestCaseSource(typeof(RanadomTestData), nameof(RanadomTestData.RandomStrings))]
		public void WithoutStartDate_ShouldFail(string title)
		{
			var args = new string[] { "add", "job", "--title", title };
			Assert.Catch(() => TestApp.Run(args));
		}

		[TestCaseSource(typeof(TestData), nameof(TestData.Dates))]
		[TestCaseSource(typeof(RanadomTestData), nameof(RanadomTestData.RandomDates))]
		public void WithoutJobTitle_ShouldFail(DateOnly startDate)
		{
			var args = new string[] { "add", "job", "--start", startDate.ToString() };
			Assert.Catch(() => TestApp.Run(args));
		}

		[TestCaseSource(typeof(AddJobTestData), nameof(AddJobTestData.GetRequiredArgs))]
		public void WithMinimumArgs_ShouldPass(string title, DateOnly startDate)
		{
			var args = new string[] { "add", "job", "--title", title, "--start", startDate.ToString() };
			Assume.That(title.Length is < 100 and > 0);
			var result = TestApp.Run(args);
			Assert.Multiple(() =>
			{
				Assert.That(result.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
				Assert.That(result.Settings, Is.InstanceOf<AddJobSettings>());
			});
		}
	}
}