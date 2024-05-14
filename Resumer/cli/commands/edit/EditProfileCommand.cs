using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.edit;

public class EditProfileCommand : Command
{
    public override int Execute(CommandContext context)
    {
        var ctx = new ResumeContext();
        var profiles = ctx.Profiles.AsEnumerable().OrderBy(p => p.ToString()).ToList();
        if(profiles.Count == 0)
            return CommandOutput.Success("No profiles found");

        var profile = AnsiConsole.Prompt(new SelectionPrompt<Profile>().Title("Select a profile to edit")
            .AddChoices(profiles));

        profile.FirstName = AnsiConsole.Prompt(new TextPrompt<string>("First name:").DefaultValue(profile.FirstName));
        profile.MiddleName = AnsiConsole.Prompt(new TextPrompt<string?>("Middle name (enter '-' to remove):")
            .DefaultValue(profile.MiddleName));
        if(profile.MiddleName == "-")
            profile.MiddleName = null;
        profile.LastName = AnsiConsole.Prompt(new TextPrompt<string>("Last name:").DefaultValue(profile.LastName));
        profile.EmailAddress = AnsiConsole.Prompt(new TextPrompt<string>("Email:").DefaultValue(profile.EmailAddress));
        profile.PhoneNumber = AnsiConsole.Prompt(new TextPrompt<string>("Phone number:").DefaultValue(profile.PhoneNumber));

        profile.Objective = AnsiConsole.Prompt(new TextPrompt<string?>("Objective:").DefaultValue(profile.Objective));
        profile.Location = AnsiConsole.Prompt(new TextPrompt<string?>("Location:").DefaultValue(profile.Location));
        profile.Website = AnsiConsole.Prompt(new TextPrompt<string?>("Website:").DefaultValue(profile.Website));
        profile.Languages.EditFromPrompt("Languages:");
        profile.Interests.EditFromPrompt("Interests:");

        ctx.SaveChanges();
        return CommandOutput.Success("Changes saved");
    }
}