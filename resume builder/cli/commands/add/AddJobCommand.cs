using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static resume_builder.Convert;

namespace resume_builder.cli.commands.add;

public class AddSetting : CommandSettings
{
}

public class AddJobSettings : AddSetting
{
	[Description("start date at the job")]
	[CommandOption("-s|--start <StartDate>")]
	public DateOnly? StartDate { get; init; } = Today;

	[Description("last date at the job")]
	[CommandOption("-e|--end")]
	public DateOnly? EndDate { get; init; }

	[Description("job title")]
	[CommandOption("-t|--title")]
	public string? JobTitle { get; init; }

	[Description("posted/official job description by the employer")]
	[CommandOption("-d|--description")]
	public string? JobDescription { get; init; }

	[Description("your (personal) experience at the job")]
	[CommandOption("-x|--experience")]
	public string? Experience { get; init; }

	[Description("The company or employer name")]
	[CommandOption("-c|--company")]
	public string? Company { get; }

	public override ValidationResult Validate()
	{
		if(string.IsNullOrWhiteSpace(JobTitle))
			if(JobTitle == null)
				return ValidationResult.Error("Job title is required");
			else
				return ValidationResult.Error("Job title is invalid: cannot be empty");

		if(EndDate < StartDate)
			return ValidationResult.Error($"end date must be after start date: {EndDate} < {StartDate}");

		return ValidationResult.Success();
	}

	public void Deconstruct(out DateOnly? startDate, out DateOnly? endDate, out string? jobTitle,
	                        out string? jobDescription, out string? experience, out string? company)
	{
		startDate = StartDate;
		endDate = EndDate;
		jobTitle = JobTitle;
		jobDescription = JobDescription;
		experience = Experience;
		company = Company;
	}
}

internal sealed class AddJobCommand : Command<AddJobSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] AddJobSettings settings)
	{
		Database database = new();
		var (startDate, endDate, jobTitle, jobDescription, experience, company) = settings;
		if(string.IsNullOrWhiteSpace(jobTitle))
			return ExitCode.InvalidArgument.ToInt();
		startDate ??= Today;

		var job = new Job(jobTitle, startDate, endDate, company, jobDescription, experience);
		database.AddJob(job);
		AnsiConsole.MarkupLine($"✅ Job \"[bold]{job.Title}[/]\" added");
		return ExitCode.Success.ToInt();
	}
}