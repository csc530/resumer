using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get.job;

public class GetJobIdCommand : Command<GetJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] GetJobSettings settings)
	{
		Database database = new();
		var jobs = database.GetJob(settings.Id!);
		var table = new Table()
			.AddColumn("Id");
		foreach(var (id, _) in jobs)
			table.AddRow(id.ToString());

		AnsiConsole.Write(table);
		return ExitCode.Success.ToInt();
	}
}