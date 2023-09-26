using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get;

public class GetJobStartDateCommand : Command<GetJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] GetJobSettings settings)
	{
		Database database = new();
		var jobs = database.GetJob(settings.Id!);
		var table = new Table()
			.AddColumn("Start Date");
		foreach(var (id, job) in jobs)
			table.AddRow(job.StartDate.ToString());

		AnsiConsole.Write(table);
		return ExitCode.Success.ToInt();
	}
}