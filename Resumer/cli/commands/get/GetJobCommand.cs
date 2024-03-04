using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetJobCommand: JobOutputCommand<GetJobCommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] GetJobCommandSettings settings)
    {
        var ids = settings.Ids ?? Array.Empty<int>();
        var rows = new Dictionary<int, Job>();
        ResumeContext database = new();
        if(!database.Jobs.Any())
        {
            AnsiConsole.MarkupLine("No jobs");
            return ExitCode.Success.ToInt();
        }

        if(ids.Length == 0)
            rows = database.Jobs
                           .AsEnumerable()
                           .Select((job, i) => new KeyValuePair<int, Job>(i, job))
                           .ToDictionary(pair => pair.Key, pair => pair.Value);
        else
            foreach(var id in ids)
                if(id < database.Jobs.Count())
                {
                    var job = database.Jobs.ElementAt(id);
                    rows.Add(id, job);
                }
                else
                {
                    AnsiConsole.MarkupLine($"Invalid job id: {id}");
                    return ExitCode.InvalidArgument.ToInt();
                }


        var jobs = rows.Values;
        if(jobs.Count == 0)
            AnsiConsole.MarkupLine("No jobs found");
        else
        {
            var table = settings.GetTable();
            if(table == null)
                PrintJobsPlain(settings, rows);
            else
                PrintJobsTable(settings, table, rows);
        }

        return ExitCode.Success.ToInt();
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