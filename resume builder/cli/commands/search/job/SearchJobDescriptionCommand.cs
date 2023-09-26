using System.Diagnostics.CodeAnalysis;
using resume_builder.cli.commands.search.job;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.Search;

public class SearchJobDescriptionCommand : Command<SearchJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] SearchJobSettings settings)
	{
		Database database = new();
		var jobs = database.GetJobsLike(settings.JobTitle, settings.StartDate, settings.EndDate, settings.Company,
			terms: settings.Terms);
		var table = new Table()
			.AddColumn("Description");
		foreach(var (id, job) in jobs)
			table.AddRow(job.Description ?? "");

		AnsiConsole.Write(table);
		return ExitCode.Success.ToInt();
	}
}