using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static Resumer.Utility;

namespace Resumer.cli.commands.add;

internal sealed class AddJobCommand: AddCommand
{
    protected override string ContinuePrompt => "Add another job?";

    protected override int AddItem(CommandContext context, AddCommandSettings settings)
    {
        var jobTitle = AnsiConsole.Ask<string>("Job Title:");

        var company = AnsiConsole.Ask<string>("Company:");

        var startDate = AnsiConsole.Prompt(new TextPrompt<DateOnly>("Start Date:").DefaultValue(Today));

        var endDatePrompt = new TextPrompt<DateOnly?>("End date:")
            .DefaultValue(null)
            .HideDefaultValue()
            .AllowEmpty()
            .Validate(date => date >= startDate
                ? ValidationResult.Success()
                : ValidationResult.Error("End date must be after start date"));
        var endDate = AnsiConsole.Prompt(endDatePrompt);

        var jobDescription = new List<string>();
        AnsiConsole.WriteLine("Enter job description(s), experience(s), or task(s)");
        jobDescription.AddFromPrompt("Job Description (point form):");


        var job = new Job(jobTitle, company)
        {
            Description = jobDescription,
            StartDate = startDate,
            EndDate = endDate,
        };

        var db = new ResumeContext();
        db.Jobs.Add(job);
        db.SaveChanges(acceptAllChangesOnSuccess: true);

        return CommandOutput.Success($"✅ Job \"[bold]{job.Title}[/]\" added");
    }
}