using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get;

public class GetJobDescriptionCommand : Command<GetJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] GetJobSettings settings)
	{
		Database database = new();
		var rows = database.GetJob(settings.Id!);
		var table = new Table()
			.AddColumn("Description");
		foreach(var (id, job) in rows)
			table.AddRow(job.Description ?? "");

		AnsiConsole.Write(table);
		return ExitCode.Success.ToInt();
	}
}