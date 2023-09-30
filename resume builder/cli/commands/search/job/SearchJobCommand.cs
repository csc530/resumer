using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using resume_builder.models.database;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.search.job;

public class SearchJobCommand : Command<SearchJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] SearchJobSettings settings)
	{
		Database database = new();
		var rows = database.GetJobsLike(settings.JobTitle, settings.StartDate, settings.EndDate, settings.Company,
			terms: settings.Terms);
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

public class SearchJobSettings : SearchCommandSettings
{
	[CommandArgument(0, "[job title]")] public string? JobTitle { get; set; }

	[CommandOption("-s|--start")]
	[Description("start date at the job")]
	public DateOnly? StartDate { get; set; }

	[CommandOption("-e|--end")]
	[Description("last date at the job")]
	public DateOnly? EndDate { get; set; }

	[CommandOption("-c|--company")]
	[Description("the company or employer name")]
	public string? Company { get; set; }

	[CommandArgument(1, "[terms]")] //[CommandOption("-t|--terms")]
	[Description("search terms to search for in the job's description, experience, or skills")]
	public string[] Terms { get; set; }

	public override ValidationResult Validate()
	{
		if(JobTitle != null && string.IsNullOrWhiteSpace(JobTitle))
			return ValidationResult.Error("Job title cannot be empty");
		if(Company != null && string.IsNullOrWhiteSpace(Company))
			return ValidationResult.Error("Company cannot be empty");
		return ValidationResult.Success();
	}
}