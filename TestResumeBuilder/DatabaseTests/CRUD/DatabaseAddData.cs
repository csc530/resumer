using resume_builder;
using TestResumeBuilder.test_data;

namespace TestResumeBuilder.DatabaseTests.CRUD;

[TestFixture]
public class DatabaseAddData : DatabaseCRUDTest
{
	[Test]
	[TestCaseSource(typeof(TestData), nameof(TestData.Jobs))]
	public void Add_Job_ShouldPass(Job job)
	{
		Assert.Multiple(() =>
		{
			Assert.That(Database.AddJob(job), Is.EqualTo(1));
			Assert.That(Database.GetJob(job), Is.Not.Empty);
		});
	}
}