using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.cli.settings;
using resume_builder.models;
using resume_builder.models.database;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.search;

public class SearchJobCommand : JobOutputCommand<SearchJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] SearchJobSettings settings)
	{
		Dictionary<long, Job> rows;
		try
		{
			Database database = new();
			rows = database.GetJobsLike(settings.Terms);
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

public class SearchJobSettings : JobOutputSettings
{
	[CommandArgument(0, "[terms]")] //[CommandOption("-t|--terms")]
	[Description("search terms to search for in the job's description, experience, or skills")]
	public string[] Terms { get; set; }
}