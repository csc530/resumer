using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.cli.settings;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.search;

public class SearchJobCommand: JobOutputCommand<SearchJobSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] SearchJobSettings settings)
    {
        Dictionary<int, Job> rows;

        try
        {
            ResumeContext database = new();
            rows = database.Jobs.AsQueryable()
                           .Where(job => settings.Terms.Any(term =>
                                    job.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                                    job.Experience.Contains(term, StringComparison.OrdinalIgnoreCase)
                                // || job.Skills.Any(skill => skill.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                            ))
                           .Select((job, i) => new KeyValuePair<int, Job>(i, job))
                           .ToDictionary(tuple => tuple.Key, tuple => tuple.Value);
        }
        catch(Exception e)
        {
            return Globals.PrintError(settings, e);
        }

        var jobs = rows;
        if(!jobs.Any())
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

public class SearchJobSettings: JobOutputSettings
{
    [CommandArgument(0, "[terms]")] //[CommandOption("-t|--terms")]
    [Description("search terms to search for in the job's description, experience, or skills")]
    public string[] Terms { get; set; }
}