using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static Resumer.Globals;

namespace Resumer.cli.commands.add;

internal sealed class AddJobCommand: Command<AddJobSettings>
{
    public override int Execute(CommandContext context, AddJobSettings settings)
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
            var jobTitlePrompt = RenderableFactory.CreateTextPrompt("Job title: ", jobTitle)
                                                  .Validate(value => string.IsNullOrWhiteSpace(value)
                                                                ? ValidationResult.Error()
                                                                : ValidationResult.Success())
                                                  .ValidationErrorMessage("[red]Job title cannot be empty[/]");
            var startDatePrompt = RenderableFactory.CreateTextPrompt("Start date: ", startDate ?? Today, true)
                                                   .Validate(date => date > Today
                                                                 ? ValidationResult.Success()
                                                                 : ValidationResult.Error())
                                                   .ValidationErrorMessage("[red]Start date must be before today[/]");
            var descriptionPrompt = RenderableFactory.CreateTextPrompt("Description: ", jobDescription);
            var experiencePrompt = RenderableFactory.CreateTextPrompt("Experience: ", experience);
            var companyPrompt = RenderableFactory.CreateTextPrompt("Company: ", company);
            var endDatePrompt = RenderableFactory.CreateTextPrompt("End date: ", endDate, true)
                                                 .Validate(date => date >= startDate
                                                               ? ValidationResult.Success()
                                                               : ValidationResult.Error())
                                                 .ValidationErrorMessage("[red]End date must be after start date[/]");

            jobTitle = AnsiConsole.Prompt(jobTitlePrompt);
            jobDescription = AnsiConsole.Prompt(descriptionPrompt);
            experience = AnsiConsole.Prompt(experiencePrompt);
            company = AnsiConsole.Prompt(companyPrompt);
            startDate = AnsiConsole.Prompt(startDatePrompt);
            endDate = AnsiConsole.Prompt(endDatePrompt);
        }

        var job = new Job
        {
            Title = jobTitle,
            Description = jobDescription,
            Experience = experience,
            Company = company,
            StartDate = (DateOnly)startDate,
            EndDate = endDate
        };

        var db = new ResumeContext();
        db.Jobs.Add(job);
        db.SaveChanges(acceptAllChangesOnSuccess: true);
        AnsiConsole.MarkupLine($"✅ Job \"[bold]{job.Title}[/]\" added");
        return CommandOutput.Success();
    }
}

public class AddJobSettings: AddCommandSettings
{
    //todo: replace prompt user with yes flag; that will fail if inputs don't match up
    public bool PromptUser => Interactive ||
                              (JobTitle.IsBlank() && Company.IsBlank() && StartDate == null && EndDate == null &&
                               JobDescription.IsBlank() && Experience.IsBlank());

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
        if(string.IsNullOrWhiteSpace(JobTitle) && !PromptUser)
            return ValidationResult.Error("Job title is invalid: cannot be empty");

        // equivalent to
        //(StartDate != null && EndDate != null && EndDate < StartDate)
        return EndDate < StartDate
            ? ValidationResult.Error($"end date must be after start date: {EndDate} !< {StartDate}")
            : ValidationResult.Success();
    }
}