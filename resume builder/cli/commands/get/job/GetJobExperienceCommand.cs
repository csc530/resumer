using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get;

public class GetJobExperienceCommand : Command<GetJobSettings>
{
	public override int Execute(CommandContext context, GetJobSettings settings)
	{
		Database database = new();
		var jobs = database.GetJobsLike(settings.JobTitle, settings.StartDate, settings.EndDate, settings.Company,
			terms: settings.Terms).Values;
		var table = new Table()
			.AddColumn("Experience");
		foreach(var job in jobs)
			table.AddRow(job.Experience ?? string.Empty);

		AnsiConsole.Write(table);
		return ExitCode.Success.ToInt();
	}
}