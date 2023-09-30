using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using resume_builder.models.database;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.search.job;

public class SearchJobEndDateCommand : Command<SearchJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] SearchJobSettings settings)
	{
		Database database = new();
		var jobs = database.GetJobsLike(settings.JobTitle, settings.StartDate, settings.EndDate, settings.Company,
			terms: settings.Terms).Values;
		var table = new Table()
			.AddColumn("End Date");
		foreach(var job in jobs)
			table.AddRow(job.EndDate.ToString() ?? string.Empty);

		AnsiConsole.Write(table);
		return ExitCode.Success.ToInt();
	}
}