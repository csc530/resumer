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
        var startDate = Today;

        var jobTitlePrompt = RenderableFactory.CreateTextPrompt<string>("Job title: ")
                                              .Validate(value => string.IsNullOrWhiteSpace(value)
                                                            ? ValidationResult.Error()
                                                            : ValidationResult.Success())
                                              .ValidationErrorMessage("[red]Job title cannot be empty[/]");
        var startDatePrompt = RenderableFactory.CreateTextPrompt<DateOnly>("Start date: ", Today, true)
                                               .Validate(date => date > Today
                                                             ? ValidationResult.Success()
                                                             : ValidationResult.Error())
                                               .ValidationErrorMessage("[red]Start date must be before today[/]");
        var descriptionPrompt = RenderableFactory.CreateTextPrompt<string?>("Description: ")
                                                 .AllowEmpty();
        var experiencePrompt = RenderableFactory.CreateTextPrompt<string>("Experience: ")
                                                .AllowEmpty();
        var companyPrompt = RenderableFactory.CreateTextPrompt<string>("Company: ");
        var endDatePrompt = RenderableFactory.CreateTextPrompt<DateOnly?>("End date: ",null,true)
                                             .HideDefaultValue()
                                             .Validate(date => date >= startDate
                                                           ? ValidationResult.Success()
                                                           : ValidationResult.Error())
                                             .ValidationErrorMessage("[red]End date must be after start date[/]");

        var jobTitle = AnsiConsole.Prompt(jobTitlePrompt);
        var jobDescription = AnsiConsole.Prompt(descriptionPrompt);
        var experience = AnsiConsole.Prompt(experiencePrompt);
        var company = AnsiConsole.Prompt(companyPrompt);
        startDate = AnsiConsole.Prompt(startDatePrompt);
        var endDate = AnsiConsole.Prompt(endDatePrompt);

        var job = new Job
        {
            Title = jobTitle,
            Description = jobDescription,
            Experience = experience,
            Company = company,
            StartDate = startDate,
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
}