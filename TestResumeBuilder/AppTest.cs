using resume_builder;
using Spectre.Console.Testing;

namespace TestResumeBuilder
{
	public class AppTest
	{
		internal Spectre.Console.Testing.CommandAppTester TestApp;

		[OneTimeSetUp]
		public virtual void InitializeApp()
		{
			TestApp = new CommandAppTester();
			TestApp.Configure(Program.AppConfiguration);
		}

		protected CommandAppResult Run(IEnumerable<string> cmdArgs, params string[] args) =>
			TestApp.Run(cmdArgs.Concat(args).ToArray());

		protected CommandAppFailure RunAndCatch<T>(IEnumerable<string> cmdArgs, params string[] args)
			where T : Exception => TestApp.RunAndCatch<T>(cmdArgs.Concat(args).ToArray());
	}
}