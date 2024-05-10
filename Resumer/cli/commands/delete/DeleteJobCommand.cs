using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.delete;

public class DeleteJobCommand:Command<DeleteCommandSettings>
{
    public override int Execute(CommandContext context, DeleteCommandSettings settings)
    {
        var db = new ResumeContext();
        var jobs = db.Jobs;

        if (!jobs.Any())
            return CommandOutput.Success("No jobs found.");
        else if (settings.DeleteAll)
        {
            if(!settings.NoConfirm && !AnsiConsole.Confirm("Are you sure you want to delete all jobs?"))
                return CommandOutput.Error(ExitCode.Canceled);
            jobs.ToList().RemoveAll(_ => true);
        }
        else
        {
            var selected = AnsiConsole.Prompt(new MultiSelectionPrompt<Job>().Title("Select job(s) to delete").AddChoices(jobs));

            AnsiConsole.WriteLine($"Deleting {selected.Count} jobs...");
            selected.ForEach(job => AnsiConsole.MarkupLine($"- {job}"));

            var prompt = selected.Count == 1
                ? $"Are you sure you want to delete {selected[0]}?"
                : $"Are you sure you want to delete these {selected.Count} jobs?";
            if(!settings.NoConfirm && !AnsiConsole.Confirm(prompt))
                return CommandOutput.Error(ExitCode.Canceled);
            jobs.RemoveRange(selected);
        }

        var deleted = db.SaveChanges();
        return CommandOutput.Success(deleted == 1 ? "1 job deleted." : $"{deleted} jobs deleted.");
    }
}