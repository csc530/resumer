using resume_builder.models;
using resume_builder.models.database;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.search.job;

public class SearchJobTitleCommand : Command<SearchJobSettings>
{
	public override int Execute(CommandContext context, SearchJobSettings settings)
	{
		Database database = new();
		var jobs = database.GetJobsLike(settings.JobTitle, settings.StartDate, settings.EndDate, settings.Company,
			terms: settings.Terms);
		var table = new Table()
			.AddColumn("Job Title");
		foreach(var (_, job) in jobs)
			table.AddRow(job.Title);

		AnsiConsole.Write(table);
		return ExitCode.Success.ToInt();
	}
}