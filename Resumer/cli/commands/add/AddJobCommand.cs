using System.Diagnostics.CodeAnalysis;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static Resumer.Globals;

namespace Resumer.cli.commands.add;

internal sealed class AddJobCommand: Command<AddJobSettings>
{
    public override int Execute([NotNull]CommandContext context, [NotNull]AddJobSettings settings)
    {
        var jobTitle = AnsiConsole.Ask<string>("Job Title:");
        var jobDescription = AnsiConsole.Prompt(new TextPrompt<string?>("Description:").AllowEmpty());
        var experience = AnsiConsole.Prompt(RenderableFactory.CreateTextPrompt<string?>("Experience:").AllowEmpty());
        var company = AnsiConsole.Ask<string>("Company:");
        var startDate = AnsiConsole.Prompt(new TextPrompt<DateOnly>("Start Date:").DefaultValue(Today));

        var endDatePrompt = RenderableFactory.CreateTextPrompt<DateOnly?>("End date:", null, true)
                                             .Validate(date => date >= startDate
                                                           ? ValidationResult.Success()
                                                           : ValidationResult.Error())
                                             .ValidationErrorMessage("End date must be after start date")
                                             .HideDefaultValue();
        var endDate = AnsiConsole.Prompt(endDatePrompt);

        var job = new Job(jobTitle, company)
        {
            Description = jobDescription,
            Experience = experience,
            StartDate = startDate,
            EndDate = endDate
        };

        var db = new ResumeContext();
        db.Jobs.Add(job);
        db.SaveChanges(acceptAllChangesOnSuccess: true);
        return CommandOutput.Success($"✅ Job \"[bold]{job.Title}[/]\" added");
    }
}

public class AddJobSettings: AddCommandSettings
{
}