using resume_builder;
using resume_builder.models;
using TestResumeBuilder.test_data;

namespace TestResumeBuilder.DatabaseTests.CRUD;

[TestFixture]
public class DatabaseAddJobTest : DatabaseCRUDTest
{
	[Test]
	[TestCaseSource(typeof(JobTestData), nameof(JobTestData.JobTitleAndStartDates))]
	public void Add_Job_WithTitleAndStartDate_ShouldPass(string title, DateOnly startDate)
	{
		var job = new Job(title, startDate);
		Assert.Multiple(() =>
		{
			Assert.That(Database.AddJob(job), Is.EqualTo(1));
			Assert.That(Database.GetJob(job), Is.Not.Null);
		});
	}
}