using resume_builder.models;
using TestResumeBuilder.test_data;

namespace TestResumeBuilder.DatabaseTests
{
	internal class DatabaseConstructorTest : DatabaseTest
	{
		[TearDown]
		public void TearDownOddPaths()
		{
			if(TestContext.CurrentContext.Test.Arguments.Length == 0)
				return;
			DisposeDatabase(); //because tear downs order aren't given ensures that there aren't any open connections to files we need to delete
			var path = (string)TestContext.CurrentContext.Test.Arguments[0]!;
			File.Delete(Path.Combine(path, ResumeSqliteFileName));
			DeleteCreatedFolders(path);
		}

		/// <summary>
		/// given a path delete all the contrived folders made by the test
		/// leaving everything as it were (not deleting parents, existing, etc.etc.)
		/// </summary>
		/// <param name="path">full or rel path to created sqlite file</param>
		// todo: needs some serious rework and readability and perf and all dat: make nice
		private static void DeleteCreatedFolders(string path)
		{
			string[] createdDirPath = Path.GetFullPath(path)
			                              .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
			                              .ToArray();

			string absCurrentDirectory = Directory.GetCurrentDirectory();
			string currentDir =
				absCurrentDirectory.Split(Path.DirectorySeparatorChar,
					StringSplitOptions.RemoveEmptyEntries)[^1];

			//plus 1 to skip the current directory
			//hence the start of the the superflously created subdirs for the tests
			int indexOfCurrDirInPwd = createdDirPath.ToList().LastIndexOf(currentDir) + 1;
			//skip base path until the start of newly created subdirs
			//if created dirs are outside/above current dir no changes are made to the path arr
			createdDirPath = createdDirPath.Skip(indexOfCurrDirInPwd).ToArray();

			for(int i = 0; i < createdDirPath.Length; i++)
			{
				//..^i: means the elements from the back/end of arr
				var dirPath = Path.GetFullPath(Path.Combine(createdDirPath[..^i]));
				//don't try and delete the current dir or the root dir
				//need absolute path to see if the dir is found someewhere within
				if(dirPath == absCurrentDirectory || dirPath == Directory.GetDirectoryRoot(absCurrentDirectory))
					break;
				Directory.Delete(dirPath);
			}
		}


		[Test]
		public void Create_Database_ShouldPass()
		{
			Assert.That(Database, Is.Not.Null);
			FileAssert.Exists(ResumeSqliteFileName, DatabaseNotFoundMessage(ResumeSqliteFileName));
		}


		[Test]
		[TestCaseSource(typeof(TestData), nameof(TestData.RelativePaths))]
		public void Create_Database_WithCustomPath_ShouldPass(string path)
		{
			Database = new Database(path);
			Assert.That(Database, Is.Not.Null);
			FileAssert.Exists(Path.Combine(path, ResumeSqliteFileName),
				DatabaseNotFoundMessage(path, ResumeSqliteFileName));
		}

		private static string DatabaseNotFoundMessage(params string[] expectedDbPath)
		{
			var fullPath = Path.GetFullPath(Path.Combine(expectedDbPath));
			return $"Database not found at {fullPath}";
		}
	}
}