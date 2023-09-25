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
		var jobs = database.SearchJobs(settings.JobTitle, settings.StartDate, settings.EndDate, settings.Company,
			terms: settings.Terms);
		if(jobs.Count == 0)
			AnsiConsole.MarkupLine("No jobs found");
		else
			foreach(var job in jobs)
				AnsiConsole.MarkupLine(job.ToString());
		return ExitCode.Success.ToInt();
	}
}

public class GetJobSettings : GetCommandSettings
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