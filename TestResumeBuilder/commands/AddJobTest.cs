using resume_builder;
using resume_builder.cli.commands.add;
using TestResumeBuilder.test_data;

namespace TestResumeBuilder.commands;

public class AddJobTest : AppTest
{
	[SetUp]
	public void Setup() => TestApp.Run("init");

	[Test]
	public void WithNoArguments_ShouldFail() => Assert.Catch(() => TestApp.Run("add", "job"));


	[Test]
	[TestCaseSource(typeof(TestData), nameof(TestData.JobTitles))]
	[TestCaseSource(typeof(RandomTestData), nameof(RandomTestData.RandomStrings))]
	public void WithoutStartDate_ShouldFail(string title) =>
		Assert.Catch(() => TestApp.Run("add", "job", "--title", title));

	[TestCaseSource(typeof(TestData), nameof(TestData.Dates))]
	[TestCaseSource(typeof(RandomTestData), nameof(RandomTestData.RandomDates))]
	public void WithoutJobTitle_ShouldFail(DateOnly startDate) =>
		Assert.Catch(() => TestApp.Run("add", "job", "--start", startDate.ToString()));

	[TestCaseSource(typeof(AddJobTestData), nameof(AddJobTestData.GetRequiredArgs))]
	public void WithMinimumArgs_ShouldPass(string title, DateOnly startDate)
	{
		Assume.That(title.Length is < 100 and > 0);
		var result = TestApp.Run("add", "job", "--title", title, "--start", startDate.ToString());
		Assert.Multiple(() =>
		{
			Assert.That(result.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
			Assert.That(result.Settings, Is.InstanceOf<AddJobSettings>());
		});
	}
}