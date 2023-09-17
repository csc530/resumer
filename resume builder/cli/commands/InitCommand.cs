using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands;

internal class InitCommand : Command
{
    private readonly Database _database = new();

    public override int Execute(CommandContext context)
    {
        if(_database.IsInitialized())
        {
            AnsiConsole.WriteLine("Database already initialized");
            //todo: provide help to clear, reset, or edit
            return ExitCode.Success.ToInt();
        }

        //prompt to copy to main
        //indicate backup found
        var recover =
            AnsiConsole.Prompt(new ConfirmationPrompt("Backup database found\nwould you like to recover from backup?"));
        if(recover)
            return _database.RestoreBackup() ? (ExitCode.Success).ToInt() : ExitCode.Fail.ToInt();
        return ExitCode.Fail.ToInt();
    }
}