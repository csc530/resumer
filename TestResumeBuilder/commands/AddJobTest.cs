using resume_builder;
using resume_builder.cli.commands.add;
using resume_builder.models;
using resume_builder.models.database;
using TestResumeBuilder.test_data;

namespace TestResumeBuilder.commands;

public class AddJobTest : AppTest
{
	[SetUp]
	public void Setup() => TestApp.Run("init");

	[TearDown]
	public void DeleteEntries()
	{
		new Database().Wipe();
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	[Test]
	[TestCaseSource(typeof(JobTestData), nameof(JobTestData.JobTitles))]
	[TestCaseSource(typeof(RandomTestData), nameof(RandomTestData.RandomStrings))]
	public void WithoutStartDate_ShouldPass(string title)
	{
		var results = TestApp.Run("add", "job", "--title", title);
		Assert.Multiple(() =>
		{
			Assert.That(results.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
			Assert.That(results.Settings, Is.InstanceOf<AddJobSettings>());
		});
		var jobs = new Database().GetJobs();
		Assert.That(jobs, Has.Count.EqualTo(1));
		var job = jobs.Values.First();
		Assert.Multiple(() =>
		{
			Assert.That(job.Title, Is.EqualTo(title));
			Assert.That(job.StartDate, Is.EqualTo(Globals.Today));
		});
	}

	[TestCaseSource(typeof(TestData), nameof(TestData.Dates))]
	[TestCaseSource(typeof(RandomTestData), nameof(RandomTestData.RandomDates))]
	public void WithoutJobTitle_ShouldFail(DateOnly startDate) =>
		Assert.Catch(() => TestApp.Run("add", "job", "--start", startDate.ToString()));

	[TestCaseSource(typeof(JobTestData), nameof(JobTestData.JobTitleAndStartDates))]
	public void WithMinimumArgs_ShouldPass(string title, DateOnly startDate)
	{
		var result = TestApp.Run("add", "job", "--title", title, "--start", startDate.ToString());
		Assert.Multiple(() =>
		{
			Assert.That(result.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
			Assert.That(result.Settings, Is.InstanceOf<AddJobSettings>());
		});
	}
}