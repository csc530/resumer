using resume_builder.models;

namespace TestResumeBuilder.commands;

public abstract class AddTest : AppTest
{
	[OneTimeSetUp]
	public override void InitializeApp()
	{
		base.InitializeApp();
		TestApp.Run("init");
	}

	[TearDown]
	public void DeleteData() => new Database().Wipe();
}