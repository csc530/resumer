using resume_builder.models;
using resume_builder.models.database;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands;

internal class InitCommand : Command
{
	public override int Execute(CommandContext context)
	{
		Database database = new();
		if(database.IsInitialized())
		{
			AnsiConsole.WriteLine("Database already initialized");
			return ExitCode.Success.ToInt();
		}

		database.Initialize();
		AnsiConsole.WriteLine("✅ Database initialized");
		return ExitCode.Success.ToInt();
	}
}