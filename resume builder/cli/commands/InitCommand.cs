using Microsoft.EntityFrameworkCore;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands;

internal class InitCommand : Command
{
	public override int Execute(CommandContext context)
	{
		ResumeContext database = new();

		if(!database.Database.CanConnect())
		{
			AnsiConsole.WriteLine("📁 Creating database");
			database.Database.Migrate();
			if(!database.Database.CanConnect())
			{
				AnsiConsole.WriteLine("❌ Error creating database");
				return ExitCode.Error.ToInt();
			}
		}
		//todo: check for existing file with the same of db and ask to overwrite or recover
		AnsiConsole.WriteLine("✅ Database initialized");
		return ExitCode.Success.ToInt();
	}
}