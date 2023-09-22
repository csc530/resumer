using NUnit.Framework.Internal;
using resume_builder;
using resume_builder.models;

namespace TestResumeBuilder;

[TestFixture]
public abstract class DatabaseTest
{
	protected const string BackupResumeSqliteFileName = "backup_resume.sqlite";
	internal const string ResumeSqliteFileName = "resume.sqlite";
	protected internal Database Database { get; set; } = new();

	[TearDown]
	public void DeleteDatabase()
	{
		if(File.Exists(ResumeSqliteFileName))
			try
			{
				File.Delete(ResumeSqliteFileName);
				// File.Delete(BackupResumeSqliteFileName);
			}
			catch(IOException e)
			{
				Assert.Warn(e.Message);
			}
		else
			Assert.Warn("Database file not found");
	}

	[SetUp]
	public virtual void SetupDatabase() => Database = new Database();

	[TearDown]
	public void DisposeDatabase()
	{
		Database.Dispose();
		// GC.Collect();
		// GC.WaitForPendingFinalizers();
	}
}