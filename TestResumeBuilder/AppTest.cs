using resume_builder;

namespace TestResumeBuilder
{
	public class AppTest
	{
		internal Spectre.Console.Testing.CommandAppTester TestApp;

		[SetUp]
		public void InitializeApp()
		{
			TestApp = new();
			TestApp.Configure(Program.AppConfiguration);
		}
	}
}