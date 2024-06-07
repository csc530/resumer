using Microsoft.EntityFrameworkCore;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.get;

public class GetProfileCommand: Command<GetProfileCommandSettings>
{
    public override int Execute(CommandContext context, GetProfileCommandSettings settings)
    {
        var profiles = new ResumeContext().Profiles;

        if(!profiles.Any())
            return CommandOutput.Success("[bold red]No[/] profiles found");
        else
        {
            var table = settings.CreateTable<Profile>("Profiles")?.AddObjects(profiles);
            if(table == null)
                profiles.ForEachAsync(profile => AnsiConsole.WriteLine(profile.ToString())).Wait();
            else
                AnsiConsole.Write(table);
            return CommandOutput.Success();
        }
    }
}

public class GetProfileCommandSettings: OutputCommandSettings;