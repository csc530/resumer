using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.delete;

public class DeleteProfileCommand : Command<DeleteCommandSettings>
{
    public override int Execute(CommandContext context, DeleteCommandSettings settings)
    {
        var db = new ResumeContext();
        var profiles = db.Profiles;
        if(!profiles.Any())
            return CommandOutput.Success("No profiles found");
        else if(settings.DeleteAll)
        {
            if(!settings.NoConfirm && !AnsiConsole.Confirm("Are you sure you want to delete all profiles?"))
                return CommandOutput.Error(ExitCode.Canceled);
            profiles.ToList().RemoveAll(_=>true);
        }
        else
        {
            var selected = AnsiConsole.Prompt(new MultiSelectionPrompt<Profile>()
                .Title("Select a profile to delete")
                .AddChoices(profiles.AsEnumerable().OrderBy(p => p.ToString()).ToList()));

            AnsiConsole.WriteLine($"Deleting {selected.Count} profiles...");
            selected.ForEach(profile => AnsiConsole.MarkupLine($"- {profile}"));

            var prompt = selected.Count == 1
                ? $"Are you sure you want to delete {selected[0]}?"
                : $"Are you sure you want to delete these {selected.Count} profiles?";
            if(!settings.NoConfirm && !AnsiConsole.Confirm(prompt))
                return CommandOutput.Error(ExitCode.Canceled);
            profiles.RemoveRange(selected);
        }

        var deleted = db.SaveChanges();
        return CommandOutput.Success(deleted == 1 ? "Profile deleted" : $"{deleted} profiles deleted");
    }
}