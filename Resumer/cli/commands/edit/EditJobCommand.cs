using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.edit;

public class EditJobCommand : Command<EditJobSettings>
{
    public override int Execute(CommandContext context, EditJobSettings settings)
    {
        var db = new ResumeContext();

        var jobs = db.Jobs.AsEnumerable().OrderBy(j => j.ToString()).ToList();
        if (jobs.Count == 0)
            return CommandOutput.Success("No jobs found");

        var job = AnsiConsole.Prompt(new SelectionPrompt<Job>().Title("Select a job to edit").AddChoices(jobs));

        job.Title = AnsiConsole.Prompt(new TextPrompt<string>("Job title:").DefaultValue(job.Title));
        job.Company = AnsiConsole.Prompt(new TextPrompt<string>("Company name:").DefaultValue(job.Company));
        job.StartDate = AnsiConsole.Prompt(new TextPrompt<DateOnly>("Start date:").DefaultValue(job.StartDate));
        job.EndDate = AnsiConsole.Prompt(new TextPrompt<DateOnly?>("End date:").DefaultValue(job.EndDate));
        job.Description.EditFromPrompt("Description (enter '-' to delete the entry):");

        db.SaveChanges();
        return CommandOutput.Success("Changes saved");
    }
}

public class EditJobSettings : CommandSettings;