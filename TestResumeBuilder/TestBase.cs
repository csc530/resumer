using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using resume_builder;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Testing;

namespace TestResumeBuilder;

[UsesVerify]
//todo:  find way to pass text to test command app for prompts
public abstract class TestBase: IDisposable, IAsyncDisposable
{
    internal readonly CommandAppTester TestApp;
    private protected readonly TestConsole TestConsole;
    internal ResumeContext TestDb;

    protected TestBase()
    {
        //given
        TestApp = new CommandAppTester(new FakeTypeRegistrar());

        TestApp.Configure(c => {
            Program.AppConfiguration(c);
            // c.ConfigureConsole(TestConsole); //? this is what spectre does inside of Run() but only  if the config is null but either way it didn't work for me 
        });
        //! I shouldn't have to do this but it doesn't work without it, the results are always empty
        //? plus I have to use TestConsole instead TestApp.Run() to get the output (.output)
        //* and it has to be after the configure because it does edit the AnsiConsole object but it doesn't work🤷🏿‍♂️
        //* even still it's sometime-ish persisting previous runs output
        TestConsole = new TestConsole();
        AnsiConsole.Console = TestConsole;
        TestDb = new ResumeContext();
        TestDb.Database.Migrate();
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

    [ModuleInitializer]
    public static void Initialize() => VerifyDiffPlex.Initialize();
}

public static class TestHelpers
{
    public static CommandAppResult Run(this CommandAppTester commandAppTester, IEnumerable<string> cmdArgs,
        params string[] args) => commandAppTester.Run(cmdArgs.Concat(args).ToArray());

    public static CommandAppResult Run(this CommandAppTester commandAppTester, string cmd, params string[] args) =>
        commandAppTester.Run(args.Prepend(cmd));

    public static CommandAppFailure RunAndCatch<T>(this CommandAppTester commandAppTester, IEnumerable<string> cmdArgs,
        params string[] args)
        where T : Exception => commandAppTester.RunAndCatch<T>(cmdArgs.Concat(args).ToArray());
}