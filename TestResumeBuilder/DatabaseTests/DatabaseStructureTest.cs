using Microsoft.Data.Sqlite;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestResumeBuilder.TestData;

namespace TestResumeBuilder.DatabaseTests
{
	internal class DatabaseStructureTest : DatabaseTest
	{
		[TearDown, SetUp]
		public void DeleteDatabase()
		{
			Database?.Dispose();
			File.Delete(ResumeSqliteFileName);
			File.Delete(BackupResumeSqliteFileName);
		}

		[Test]
		public void InitDatabase_ShouldPass()
		{
			Database = new();
			Database.Initialize();
			Assert.Multiple(() =>
			{
				Assert.That(File.Exists((BackupResumeSqliteFileName)));
				Assert.That(File.Exists((ResumeSqliteFileName)));
				Assert.That(Database.IsInitialized());
			});
		}

		[Test]
		public void IsInitialized_WhenUninitialized_ShouldFail()
		{
			Database = new();
			Assert.That(!Database.IsInitialized());
		}

		[Test]
		public void IsInitialized_WhenInitialized_ShouldPass()
		{
			Database = new();
			Database.Initialize();
			Assert.That(Database.IsInitialized());
		}

		[Test]
		public void Wipe_PopulatedTable_ShouldPass()
		{
			Database = new();
			Database.Initialize();
			Assume.That(Database.IsInitialized());
			var rnd = new Randomizer(((int)DateTime.UtcNow.Ticks));
			var date = RandomData.RandomDate;
			for(int i = 0; i < rnd.Next(100) + 1; i++)
				Database.AddJob(new(rnd.GetString(), date,
					rnd.NextBool() ? date.AddDays(rnd.Next(10)) : null,
					rnd.NextBool() ? rnd.GetString() : null,
					rnd.NextBool() ? rnd.GetString() : null,
					rnd.NextBool() ? rnd.GetString() : null));
			Database.Wipe();
			Assert.That(Database.GetJobs(), Is.Empty);
		}
	}
}