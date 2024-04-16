using System.Diagnostics.CodeAnalysis;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetProfileCommand: Command<OutputCommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] OutputCommandSettings settings)
    {
        var profiles = new ResumeContext().Profiles;

        if(!profiles.Any())
            return CommandOutput.Success("[bold red]No[/] profiles found");
        else
        {
            var table = new Table();
            table.AddColumns("Name", "Email", "Phone", "Location", "Interests", "Objective", "Languages", "Website");

            foreach(var profile in profiles)
                table.AddRow(profile.WholeName,
                    profile.EmailAddress,
                    profile.PhoneNumber,
                    profile.Location ?? "",
                    string.Join(", ", profile.Interests),
                    profile.Objective ?? "",
                    string.Join(", ", profile.Languages),
                    profile.Website ?? "");

            AnsiConsole.Write(table);

            return CommandOutput.Success();
        }
    }
}