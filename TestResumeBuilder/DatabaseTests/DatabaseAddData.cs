using resume_builder;

namespace TestResumeBuilder.DatabaseTests;

[TestFixture]
public class DatabaseAddData : DatabaseTest
{
	[Test]
	[TestCaseSource(typeof(TestData), nameof(TestData.Jobs))]
	public void Add_Job_ShouldPass(Job job)
	{
		Assert.IsTrue(Database.AddJob(job));
		Assert.IsNotEmpty(Database.GetJob(job));
	}
}