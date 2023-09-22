using resume_builder.models;

namespace TestResumeBuilder.DatabaseTests.CRUD
{
	public abstract class DatabaseCRUDTest : DatabaseTest
	{
		[SetUp]
		public override void SetupDatabase()
		{
			Database = new Database();
			Database.Initialize();
		}
	}
}