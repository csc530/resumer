using TestResumeBuilder.test_data;

namespace TestResumeBuilder.DatabaseTests
{
	internal class DatabaseStructureTest : DatabaseTest
	{
		[Test]
		public void InitDatabase_ShouldPass()
		{
			Database.Initialize();
			Assert.Multiple(() =>
			{
				Assert.That(File.Exists((ResumeSqliteFileName)));
				Assert.That(Database.IsInitialized());
			});
		}

		[Test]
		public void IsInitialized_WhenUninitialized_ShouldFail() => Assert.That(!Database.IsInitialized());

		[Test]
		public void IsInitialized_WhenInitialized_ShouldPass()
		{
			Database.Initialize();
			Assert.That(Database.IsInitialized());
		}

		[Test]
		public void Wipe_PopulatedTable_ShouldPass()
		{
			Database.Initialize();
			Assume.That(Database.IsInitialized());

			for(int i = 0; i < JobTestData.Jobs.Length; i++)
				Database.AddJob(JobTestData.Jobs[i]);
			Database.Wipe();
			Assert.That(Database.GetJobs(), Is.Empty);
		}

		[Test]
		public void Wipe_UnpopulatedTable_ShouldPass()
		{
			Database.Initialize();
			Assume.That(Database.IsInitialized());
			Database.Wipe();
			Assert.That(Database.GetJobs(), Is.Empty);
		}

		[Test]
		public void Wipe_UnInitialized_ShouldPass()
		{
			Assume.That(!Database.IsInitialized());
			Assert.DoesNotThrow(() => Database.Wipe());
		}
	}
}