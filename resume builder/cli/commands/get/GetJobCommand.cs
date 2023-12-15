using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.cli.settings;
using resume_builder.models;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get.job;

public class GetJobCommand : JobOutputCommand<GetJobCommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] GetJobCommandSettings settings)
    {
        var ids = settings.Ids ?? Array.Empty<long>();
        Dictionary<long, Job> rows;
        try
        {
            ResumeContext database = new();
            rows = database.Jobs.Where((job, i) => ids.Contains(i))
                .Select((job, i) => new KeyValuePair<long, Job>(i, job))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

        }
        catch(Exception e)
        {
            return Globals.PrintError(settings, e);
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

public class GetJobCommandSettings : JobOutputSettings
{
    [CommandArgument(0, "[id]")]
    [Description("id(s) of jobs to retrieve")]
    public long[]? Ids { get; set; }

    public override ValidationResult Validate()
    {
        return Ids != null && Array.Exists(Ids, id => id == null || id < 0)
            ? ValidationResult.Error("id must be zero (0) or a positive number")
            : ValidationResult.Success();
    }
}