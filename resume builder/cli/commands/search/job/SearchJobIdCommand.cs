using System.Diagnostics.CodeAnalysis;
using resume_builder.cli.commands.search.job;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.Search.job;

public class SearchJobIdCommand : Command<SearchJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] SearchJobSettings settings)
	{
		Database database = new();
		var jobs = database.GetJobsLike(settings.JobTitle, settings.StartDate, settings.EndDate, settings.Company,
			terms: settings.Terms);
		var table = new Table()
			.AddColumn("Id");
		foreach(var (id, _) in jobs)
			table.AddRow(id.ToString());

		AnsiConsole.Write(table);
		return ExitCode.Success.ToInt();
	}
}