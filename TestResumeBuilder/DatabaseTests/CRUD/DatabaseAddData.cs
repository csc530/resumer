using resume_builder;

namespace TestResumeBuilder.DatabaseTests.CRUD;

[TestFixture]
public class DatabaseAddData : DatabaseCRUDTest
{
	[Test]
	[TestCaseSource(typeof(TestData), nameof(TestData.Jobs))]
	public void Add_Job_ShouldPass(Job job)
	{
		Assert.That(Database.AddJob(job));
		Assert.IsNotEmpty(Database.GetJob(job));
	}
}