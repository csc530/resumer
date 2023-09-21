using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands;

internal class InitCommand : Command
{
	public override int Execute(CommandContext context)
	{
		Database database = new();
		//todo: add help and fails
		if(database.IsInitialized())
		{
			AnsiConsole.WriteLine("Database already initialized");
			//todo: provide help to clear, reset, or edit
			return ExitCode.Success.ToInt();
		}

		//prompt to copy to main
		//indicate backup found
		if(!database.BackupExists())
		{
			database.Initialize();
			AnsiConsole.WriteLine("✅Database initialized");
			//TODO: add prompt for basic information populaation
			return ExitCode.Success.ToInt();
		}

		var recover =
			AnsiConsole.Prompt(new ConfirmationPrompt("Backup database found\nwould you like to recover from backup?"));
		if(recover)
			database.RestoreBackup();
		return (ExitCode.Success).ToInt();
	}
}