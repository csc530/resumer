using System.Diagnostics.CodeAnalysis;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetProfileCommand: Command<OutputCommandSettings>
{
    public override int Execute(CommandContext context, OutputCommandSettings settings)
    {
        var profiles = new ResumeContext().Profiles;

        if(!profiles.Any())
            return CommandOutput.Success("[bold red]No[/] profiles found");

        var table = new Table();
        table.AddColumns("Name", "Email", "Phone", "Summary", "Website");

        foreach(var profile in profiles)
            table.AddRow(profile.WholeName, profile.EmailAddress, profile.PhoneNumber,
                profile.Objective.GetPrintValue(),
                profile.Website.GetPrintValue());
        AnsiConsole.Write(table);

        return CommandOutput.Success();
    }
}