using Microsoft.EntityFrameworkCore;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetJobCommand: Command<GetJobCommandSettings>
{
    /// <inheritdoc />
    public override int Execute(CommandContext context, GetJobCommandSettings settings)
    {
        ResumeContext database = new();

        if(!database.Jobs.Any())
            return CommandOutput.Success("No jobs found");
        else
        {
            var table = settings.CreateTable<Job>("Jobs")?.AddObjects(database.Jobs);
            if(table == null)
                database.Jobs.ForEachAsync(job => AnsiConsole.WriteLine(job.ToString())).Wait();
            else
                AnsiConsole.Write(table);
            return CommandOutput.Success();
        }
    }
}

public class GetJobCommandSettings: OutputCommandSettings { }