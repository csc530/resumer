using resume_builder.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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