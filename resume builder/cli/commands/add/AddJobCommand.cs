using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using resume_builder.models;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static resume_builder.Globals;

namespace resume_builder.cli.commands.add;

internal sealed class AddJobCommand : Command<AddJobSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] AddJobSettings settings)
    {
        //? prompt user for required fields
        var jobTitle = settings.JobTitle;
        var jobDescription = settings.JobDescription;
        var experience = settings.Experience;
        var startDate = settings.StartDate;
        var company = settings.Company;
        var endDate = settings.EndDate;

        if(settings.PromptUser)
        {
            // todo: show entered fields as default
            var jobTitlePrompt = new TextPrompt<string>("Job title: ")
            {
                ShowDefaultValue = false,
                AllowEmpty = false,
                Validator = value => string.IsNullOrWhiteSpace(value)
                    ? ValidationResult.Error("Job title is invalid: cannot be empty")
                    : ValidationResult.Success(),
            };
            var descriptionPrompt = new TextPrompt<string?>("Description: ").ShowDefaultValue(false).AllowEmpty();
            var experiencePrompt = new TextPrompt<string?>("Experience: ").ShowDefaultValue(false).AllowEmpty();
            var companyPrompt = new TextPrompt<string?>("Company: ").ShowDefaultValue(false).AllowEmpty();
            var endDatePrompt = new TextPrompt<DateOnly?>("End date: ")
                                .AllowEmpty()
                                .DefaultValue(null)
                                .ShowDefaultValue(false)
                                .Validate(date => date > startDate
                                    ? ValidationResult.Success()
                                    : ValidationResult.Error("[red]End date must be after start date[/]"));

            jobTitle = AnsiConsole.Prompt(jobTitlePrompt);
            jobDescription = AnsiConsole.Prompt(descriptionPrompt);
            experience = AnsiConsole.Prompt(experiencePrompt);
            company = AnsiConsole.Prompt(companyPrompt);
            startDate = AnsiConsole.Ask("Start date: ", Today);
            endDate = AnsiConsole.Prompt(endDatePrompt);
        }

        if(string.IsNullOrWhiteSpace(jobTitle))
            return PrintError(ExitCode.InvalidArgument, "Job title is invalid, it cannot be empty");

        try
        {
            var job = new Job()
            {
                Title = jobTitle,
                Description = jobDescription,
                Experience = experience,
                Company = company,
                StartDate = (DateOnly)startDate!,
                EndDate = endDate
                
            };
            ResumeContext db = new ResumeContext();
            db.Jobs.Add(job);
            db.SaveChanges(acceptAllChangesOnSuccess: true);
            AnsiConsole.MarkupLine($"✅ Job \"[bold]{job.Title}[/]\" added");
            return ExitCode.Success.ToInt();
        }
        catch(Exception e)
        {
            return PrintError(settings, e);
        }
    }
}
public class AddJobSettings : AddCommandSettings
{
    public bool PromptUser =>
        (JobTitle.IsBlank() && Company.IsBlank() && StartDate == null && EndDate == null &&
         JobDescription.IsBlank() && Experience.IsBlank())
        ||
        //? why did i do this? shouldn't only be if they're all null???
        (!JobTitle.IsBlank() && !Company.IsBlank() && StartDate != null && EndDate != null &&
         !JobDescription.IsBlank() && !Experience.IsBlank())
        ||
        Interactive;

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
        if(string.IsNullOrWhiteSpace(JobTitle) && JobTitle != null ||
           (string.IsNullOrWhiteSpace(JobTitle) && !PromptUser))
            return ValidationResult.Error("Job title is invalid: cannot be empty");

        return EndDate < StartDate
            ? //(StartDate != null && EndDate < StartDate)
            ValidationResult.Error($"end date must be after start date: {EndDate} < {StartDate}")
            : ValidationResult.Success();
    }
}
