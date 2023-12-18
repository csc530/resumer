using resume_builder;
using resume_builder.models;
using Spectre.Console.Testing;

namespace TestResumeBuilder
{
    //todo:  find way to pass text to test command app for prompts
    public abstract class TestBase: IDisposable, IAsyncDisposable
    {
        internal CommandAppTester TestApp;
        internal ResumeContext TestDb;

        public TestBase()
        {
            //given
            TestApp = new CommandAppTester();
            TestApp.Configure(Program.AppConfiguration);
            TestDb = new ResumeContext();
        }

        public async ValueTask DisposeAsync()
        {
            TestDb.Jobs.RemoveRange(TestDb.Jobs);
            TestDb.Projects.RemoveRange(TestDb.Projects);
            TestDb.Profiles.RemoveRange(TestDb.Profiles);
            TestDb.Companies.RemoveRange(TestDb.Companies);
            TestDb.Skills.RemoveRange(TestDb.Skills);
            await TestDb.SaveChangesAsync();
            await TestDb.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            TestDb.Jobs.RemoveRange(TestDb.Jobs);
            TestDb.Projects.RemoveRange(TestDb.Projects);
            TestDb.Profiles.RemoveRange(TestDb.Profiles);
            TestDb.Companies.RemoveRange(TestDb.Companies);
            TestDb.Skills.RemoveRange(TestDb.Skills);
            TestDb.SaveChanges();
            TestDb.Dispose();

            GC.SuppressFinalize(this);
        }

        protected CommandAppResult Run(IEnumerable<string> cmdArgs, params string[] args) =>
            TestApp.Run(cmdArgs.Concat(args).ToArray());

        protected CommandAppResult Run(string cmd, params string[] args) =>
            Run(args.Prepend(cmd));

        protected CommandAppFailure RunAndCatch<T>(IEnumerable<string> cmdArgs, params string[] args)
            where T : Exception => TestApp.RunAndCatch<T>(cmdArgs.Concat(args).ToArray());
    }
}