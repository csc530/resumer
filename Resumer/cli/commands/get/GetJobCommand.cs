using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetJobCommand: JobOutputCommand<GetJobCommandSettings>
{
    public override int Execute(CommandContext context, GetJobCommandSettings settings)
    {
        var rows = new Dictionary<int, Job>();
        ResumeContext database = new();
        var ids = settings.Ids ?? database.Jobs.AsEnumerable().Select((_, i) => i).ToArray();

        if(!database.Jobs.Any())
            return CommandOutput.Success("No jobs found");
        else
        {
            foreach(var id in ids)
            {
                Job job;
                if(id < database.Jobs.Count() && id >= 0)
                    job = database.Jobs.ElementAt(id);
                else if(id < 0 && id >= -database.Jobs.Count())
                    job = database.Jobs.ElementAt(database.Jobs.Count() + id);
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Invalid job id: {id}[/]");
                    continue;
                }
                rows.Add(id, job);
            }


            var table = settings.GetTable();
            if(rows.Values.Count == 0)
                AnsiConsole.MarkupLine("No jobs found");
            else if(table == null)
                PrintJobsPlain(settings, rows);
            else
                PrintJobsTable(settings, table!, rows);

            return CommandOutput.Success();
        }
    }
}

public class GetJobCommandSettings: JobOutputSettings
{
    [CommandArgument(0, "[id]")]
    [Description("id(s) of jobs to retrieve")]
    public int[]? Ids { get; set; }

    public override ValidationResult Validate()
    {
        return Ids != null && Array.Exists(Ids, id => id < 0)
            ? ValidationResult.Error("id must be greater than or equal to zero (0)")
            : ValidationResult.Success();
    }
}