using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static resume_builder.Globals;

namespace resume_builder.cli.commands.get;

public class GetJobCommand : Command<GetJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] GetJobSettings settings)
	{
		Database database = new();
		var rows = database.GetJob(settings.Id ?? Array.Empty<long>());
		var jobs = rows.Values;
		if(jobs.Count == 0)
			AnsiConsole.MarkupLine("No jobs found");
		else
		{
			var table = new Table()
			            .AddColumn(new("ID"))
			            .AddColumn(new("Job Title"))
			            .AddColumn(new("Company"))
			            .AddColumn(new("Start Date"))
			            .AddColumn(new("End Date"))
			            .AddColumn(new("Description"))
			            .AddColumn(new("Experience"))
			            .Centered()
			            .Border(TableBorder.Rounded)
			            .Expand();


			foreach(var (id, job) in rows)
				table.AddRow(id.ToString(), job.Title, job.Company ?? "", job.StartDate.ToString(),
					job.EndDate.ToString() ?? "", job.Description ?? "", job.Experience ?? "");

			AnsiConsole.Write(table);
		}

		return ExitCode.Success.ToInt();
	}
}

public class GetJobSettings : GetCommandSettings
{
	[CommandArgument(0, "[id]")]
	[Description("id(s) of jobs to retrieve")]
	public long[]? Id { get; set; }
	//todo: add discriminators/options for each field indicating what to return

	public override ValidationResult Validate()
	{
		return Id != null && Id.Any(id => id == null || id < 0)
			? ValidationResult.Error("id must be zero (0) or a positive number")
			: ValidationResult.Success();
	}
}