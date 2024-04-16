using System.Diagnostics.CodeAnalysis;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static Resumer.Globals;

namespace Resumer.cli.commands.add;

internal sealed class AddJobCommand : Command<AddJobSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] AddJobSettings settings)
    {
        var jobTitle = AnsiConsole.Ask<string>("Job Title:");
        var jobDescription = new List<string>();
        jobDescription.AddFromPrompt("Job Description (point form):");

        var experience = new List<string>();
        experience.AddFromPrompt("Experience (point form):");

        var company = AnsiConsole.Ask<string>("Company:");
        var startDate = AnsiConsole.Prompt(new TextPrompt<DateOnly>("Start Date:").DefaultValue(Today));

        var endDatePrompt = new TextPrompt<DateOnly?>("End date:")
            .HideDefaultValue()
            .DefaultValue(null)
            .AllowEmpty()
            .Validate(date => date >= startDate
                ? ValidationResult.Success()
                : ValidationResult.Error("End date must be after start date"));
        var endDate = AnsiConsole.Prompt(endDatePrompt);

        var job = new Job(jobTitle, company)
        {
            Description =  jobDescription,
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

public class AddJobSettings : AddCommandSettings;