using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.delete;

public class DeleteSkillCommand : Command<DeleteCommandSettings>
{
    public override int Execute(CommandContext context, DeleteCommandSettings settings)
    {
        var db = new ResumeContext();
        var skills = db.Skills;
        if(!skills.Any())
            return CommandOutput.Success("No skills found");
        else if(settings.DeleteAll)
        {
            if(!settings.NoConfirm && !AnsiConsole.Confirm($"Are you sure you want to delete all {skills.Count()} skills?", false))
                return CommandOutput.Error(ExitCode.Canceled);
            skills.ToList().RemoveAll(_ => true);
        }
        else
        {
            var selected = AnsiConsole.Prompt(new MultiSelectionPrompt<Skill>().Title("Select skills to delete")
                .PageSize(10).AddChoices(skills));
            var prompt = selected.Count == 1
                ? $"Are you sure you want to delete this skill - {selected[0]}?"
                : $"Are you sure you want to delete these {selected.Count} skills?";
            if(!settings.NoConfirm && !AnsiConsole.Confirm(prompt))
                return CommandOutput.Error(ExitCode.Canceled);
            skills.RemoveRange(selected);
        }

        var deleted = db.SaveChanges();
        return CommandOutput.Success(deleted == 1 ? "Skill deleted" : $"{deleted} skills deleted");
    }
}