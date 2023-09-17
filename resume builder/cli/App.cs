using resume_builder.cli.commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli;

public sealed partial class App
{


    private static int ReturnCode(ExitCode exitcode) => (int)exitcode;


    public App(IAnsiConsole? console = null)
    {
        if(console != null)
            AnsiConsole.Console = console;
    }

    public int Run(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
        #if DEBUG
            config.PropagateExceptions();
            config.ValidateExamples();
        #endif
            config.SetApplicationName("resume builder");
            config.SetApplicationVersion("1.0.0");
            config.CaseSensitivity(CaseSensitivity.None);

            config.AddBranch<commands.add.App.AddSetting>("add", add =>
            {
                add.SetDescription("add new information to job database/bank");
                add.AddCommand<commands.add.App.AddJobCommand>("job")
                    .WithDescription("add a new job")
                    .WithExample("add", "job", "-s", "2022-01-01", "-e", "2026-11-01", "-t", "foreman");
            });
            config.AddCommand<InitCommand>("init")
                .WithDescription("initializes resume database")
                .WithAlias("start");
        });

        int exitCode = app.Run(args);
        return exitCode;
    }

}