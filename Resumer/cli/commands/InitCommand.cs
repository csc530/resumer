using Microsoft.EntityFrameworkCore;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands;

internal class InitCommand : Command
{
	public override int Execute(CommandContext context)
	{
		var db = new ResumeContext().Database;
		if(db.GetPendingMigrations().Any() || !db.CanConnect())
		{
			AnsiConsole.WriteLine("📁 Creating database");
			db.Migrate();
			if(!db.CanConnect())
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