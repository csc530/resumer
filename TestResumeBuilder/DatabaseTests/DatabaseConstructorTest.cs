namespace TestResumeBuilder.DatabaseTests
{
	internal class DatabaseConstructorTest : DatabaseTest
	{
		[TearDown]
		public void TearDownOddPaths()
		{
			// Database.Close();
			Database.Dispose();
			// Database = null;

			// - use one of the above statements
			//so the files are deleted; some wierd blocking file in process bug or sumn (https://stackoverflow.com/a/374595/16929246)
			// wierd

			//**********************************************
			GC.Collect();
			GC.WaitForPendingFinalizers();
			//**********************************************/
			if(TestContext.CurrentContext.Test.Arguments.Length == 0 ||
			   TestContext.CurrentContext.Test.Arguments[0] is null ||
			   TestContext.CurrentContext.Test.Arguments[0] is not string)
			{
				File.Delete((ResumeSqliteFileName));
				File.Delete((BackupResumeSqliteFileName));
			}
			else
			{
				var path = (string)TestContext.CurrentContext.Test.Arguments[0]!;
				File.Delete(Path.Combine(path, ResumeSqliteFileName));
				File.Delete(Path.Combine(path, BackupResumeSqliteFileName));

				DeleteCreatedFolders(path);
			}
		}

		/// <summary>
		/// given a path delete all the contrived folders made by the test
		/// leaving everything as it were (not deleting parents, existing, etc.etc.)
		/// </summary>
		/// <param name="path">full or rel path to created sqlite file</param>
		/// todo: needs some serious reqork and readability and perf and all dat
		private static void DeleteCreatedFolders(string path)
		{
			var fullCurrentDir = Directory.GetCurrentDirectory();

			var createdDirPath = Path.GetFullPath(path)
			                         .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
			                         .ToArray();

			var currentDir = fullCurrentDir.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
			                               .Last(); //[^1];

			//plus 1 to skip the current directory hence the start of the thesuperflously created subdirs for the tests
			var indexOfCurrDirInPwd = createdDirPath.ToList().LastIndexOf(currentDir) + 1;

			createdDirPath = createdDirPath.Skip(indexOfCurrDirInPwd)
			                               .ToArray();

			for(int i = 0; i < createdDirPath.Length; i++)
			{
				//..^i: means the elements from the back/end of arr
				var dirPath = Path.GetFullPath(Path.Combine(createdDirPath[..^i]));
				if(dirPath == fullCurrentDir || dirPath == Directory.GetDirectoryRoot(fullCurrentDir))
					break;
				Directory.Delete(dirPath);
			}
		}


		[Test]
		public void Create_Database_ShouldPass()
		{
			Database = new();
			Assert.That(Database, Is.Not.Null);
			FileAssert.Exists(ResumeSqliteFileName, DatabaseNotFoundMessage(ResumeSqliteFileName));
			FileAssert.Exists(BackupResumeSqliteFileName, DatabaseNotFoundMessage(BackupResumeSqliteFileName));
		}


		[Test]
		[TestCaseSource(typeof(TestData),nameof(TestData.Paths))]
		[Category("oddPaths")]
		public void Create_Database_WithCustomPath_ShouldPass(string path)
		{
			Database = new(path);
			Assert.That(Database, Is.Not.Null);
			FileAssert.Exists(Path.Combine(path, ResumeSqliteFileName),
				DatabaseNotFoundMessage(path, ResumeSqliteFileName));
			FileAssert.Exists(Path.Combine(path, BackupResumeSqliteFileName),
				DatabaseNotFoundMessage(path, BackupResumeSqliteFileName));
		}

		private static string DatabaseNotFoundMessage(params string[] expectedDbPath)
		{
			var expectedPath = Path.Combine(expectedDbPath);
			var combinedExpectedPath = Path.Combine(expectedPath, BackupResumeSqliteFileName);
			var fullPath = Path.GetFullPath(combinedExpectedPath);
			return $"Database not found at {fullPath}";
		}
	}
}