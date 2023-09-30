using resume_builder.models;

namespace TestResumeBuilder.commands;

public abstract class AddTest : AppTest
{
	protected internal const string ResumeSqliteFileName = "resume.sqlite";

	[OneTimeSetUp]
	public override void InitializeApp()
	{
		base.InitializeApp();
		TestApp.Run("init");
	}

	[TearDown]
	public void DeleteData()
	{
		GC.WaitForPendingFinalizers();
		if(File.Exists(ResumeSqliteFileName))
			try
			{
				File.Delete(ResumeSqliteFileName);
			}
			catch(IOException e)
			{
				Assert.Warn(e.Message);
			}
		else
			Assert.Warn("Database file not found/deleted");
	}
}