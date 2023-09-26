using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static resume_builder.Globals;

namespace resume_builder.cli.commands.add;

public class AddJobSettings : AddCommandSettings
{
	[Description("start date at the job")]
	[CommandOption("-s|--start <StartDate>")]
	public DateOnly? StartDate { get; set; }

	[Description("last date at the job")]
	[CommandOption("-e|--end")]
	public DateOnly? EndDate { get; set; }

	[Description("job title")]
	[CommandOption("-t|--title")]
	public string? JobTitle { get; set; }

	[Description("posted/official job description by the employer")]
	[CommandOption("-d|--description")]
	public string? JobDescription { get; set; }

	[Description("your (personal) experience at the job")]
	[CommandOption("-x|--experience")]
	public string? Experience { get; set; }

	[Description("The company or employer name")]
	[CommandOption("-c|--company")]
	public string? Company { get; set; }

	public override ValidationResult Validate()
	{
		if(string.IsNullOrWhiteSpace(JobTitle) && JobTitle != null)
			return ValidationResult.Error("Job title is invalid: cannot be empty");

		if(StartDate != null && EndDate < StartDate)
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
		var (startDate, endDate, jobTitle, jobDescription, experience, company) = settings;
		if((jobDescription == null && experience == null && company == null && jobTitle == null && endDate == null &&
		    startDate == null) || settings.Interactive)
		{
			var prompt = new TextPrompt<string>("[bold]*[/]Job title: ")
			             .ShowDefaultValue(false)
			             .Validate(value => string.IsNullOrWhiteSpace(value)
				             ? ValidationResult.Error("[red]Job title cannot be empty[/]")
				             : ValidationResult.Success());
			var descriptionPrompt = new TextPrompt<string?>("Job description: ") { ShowDefaultValue = false }
			                        .DefaultValue(null)
			                        .AllowEmpty();
			var experiencePrompt = descriptionPrompt.Clone("Experience: ");
			var companyPrompt = experiencePrompt.Clone("Company: ");
			var endDatePrompt = new TextPrompt<DateOnly?>("End date: ")
			                    .AllowEmpty()
			                    .DefaultValue(null)
			                    .ShowDefaultValue(false)
			                    .Validate(date => date > startDate
				                    ? ValidationResult.Success()
				                    : ValidationResult.Error("[red]End date must be after start date[/]"));


			jobTitle = AnsiConsole.Prompt(prompt);
			jobDescription = AnsiConsole.Prompt(descriptionPrompt);
			experience = AnsiConsole.Prompt<string?>(experiencePrompt);
			company = AnsiConsole.Prompt<string?>(companyPrompt);
			startDate = AnsiConsole.Ask("[bold]*[/]Start date: ", Today);
			endDate = AnsiConsole.Prompt(endDatePrompt);
		}

		Database database = new();
		if(string.IsNullOrWhiteSpace(jobTitle))
			return ExitCode.InvalidArgument.ToInt();
		startDate ??= Today;

		var job = new Job(jobTitle, startDate, endDate, company, jobDescription, experience);
		database.AddJob(job);
		AnsiConsole.MarkupLine($"✅ Job \"[bold]{job.Title}[/]\" added");
		return ExitCode.Success.ToInt();
	}
}