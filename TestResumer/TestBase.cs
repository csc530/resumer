using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Testing;

namespace TestResumer;

//todo:  find way to pass text to test command app for prompts
//todo: fix db concurrency errors for tests and errs/bugs
public abstract class TestBase
{
    internal readonly CommandAppTester TestApp;
    private protected readonly TestConsole TestConsole;
    internal ResumeContext TestDb { get; }

    protected TestBase()
    {
        //given
        TestApp = new CommandAppTester();
        TestApp.Configure(c => {
                              Resumer.Program.AppConfiguration(c);
                              // c.ConfigureConsole(TestConsole); //? this is what spectre does inside of Run() but only  if the config is null but either way it didn't work for me
                          });
        //! I shouldn't have to do this but it doesn't work without it, the results are always empty
        //? plus I have to use TestConsole instead TestApp.Run() to get the output (.output)
        //* and it has to be after the configure because it does edit the AnsiConsole object but it doesn't work🤷🏿‍♂️
        //* even still it's sometime-ish persisting previous runs output
        TestConsole = new TestConsole();
        AnsiConsole.Console = TestConsole;
        TestDb = new ResumeContext();
    }

    ~TestBase()
    {
        TestDb.Profiles.RemoveRange(TestDb.Profiles);
        TestDb.Jobs.RemoveRange(TestDb.Jobs);
        TestDb.Projects.RemoveRange(TestDb.Projects);
        TestDb.Skills.RemoveRange(TestDb.Skills);
        TestDb.SaveChanges();
    }
}

public static class TestHelpers
{
    public static CommandAppResult Run(this CommandAppTester commandAppTester, IEnumerable<string> cmdArgs,
        params string[] args) => commandAppTester.Run(cmdArgs.Concat(args).ToArray());

    public static CommandAppResult Run(this CommandAppTester commandAppTester, string cmd, params string[] args) =>
        commandAppTester.Run(args.Prepend(cmd));

    public static CommandAppFailure RunAndCatch<T>(this CommandAppTester commandAppTester,
        IEnumerable<string> cmdArgs,
        params string[] args)
        where T : Exception => commandAppTester.RunAndCatch<T>(cmdArgs.Concat(args).ToArray());

    public static int ToInt(this ExitCode exitCode) => (int)exitCode;
}