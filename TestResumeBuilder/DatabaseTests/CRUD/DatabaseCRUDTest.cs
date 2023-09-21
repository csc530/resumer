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
		public void InitDatabase()
		{
			Database = new Database();
			Database.Initialize();
		}

		[TearDown]
		public void DisposeDatabase()
		{
			Database.Wipe();
			Database.Dispose();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}