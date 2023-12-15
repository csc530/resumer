using resume_builder;
using Spectre.Console.Testing;

namespace TestResumeBuilder
{
    //todo:  find way to pass text to test command app for prompts
    public abstract class AppTest
    {
        internal CommandAppTester TestApp;

        [SetUp]
        public virtual void InitializeApp()
        {
            TestApp = new CommandAppTester();
            TestApp.Configure(Program.AppConfiguration);
        }


        protected CommandAppResult Run(IEnumerable<string> cmdArgs, params string[] args) =>
            TestApp.Run(cmdArgs.Concat(args).ToArray());

        protected CommandAppResult Run(string cmd, params string[] args) =>
            Run(args.Prepend(cmd));

        protected CommandAppFailure RunAndCatch<T>(IEnumerable<string> cmdArgs, params string[] args)
            where T : Exception => TestApp.RunAndCatch<T>(cmdArgs.Concat(args).ToArray());
    }
}