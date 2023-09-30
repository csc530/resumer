using resume_builder.models;
using resume_builder.models.database;

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