using Microsoft.EntityFrameworkCore;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static Resumer.Globals;

namespace Resumer.cli.commands;

//todo: add or suggest init call when db is out of date; in instance of an app update
internal class InitCommand : Command
{
    public override int Execute(CommandContext context)
    {
        var db = new ResumeContext().Database;
        if(db.GetPendingMigrations().Any() || !db.CanConnect())
        {
            AnsiConsole.WriteLine("📁 Creating database");
            if(!Path.Exists(new ResumeContext().DbPath))
                File.Create(new ResumeContext().DbPath);
            db.Migrate();
        }

        return !db.CanConnect() ? CommandOutput.Error(ExitCode.DbError, "❌ Error creating database") :
            //todo: check for existing file with the same of db and ask to overwrite or recover
            CommandOutput.Success($"✅ Database initialized");
    }
}