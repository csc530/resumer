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
        var table = new Table();
        table.AddColumns("Name", "Email", "Phone", "Summary", "Website");
        foreach(var profile in profiles)
            table.AddRow(profile.WholeName, profile.EmailAddress, profile.PhoneNumber, profile.Summary.GetPrintValue(),
                profile.Website.GetPrintValue());
        AnsiConsole.Write(table);
        if(!profiles.Any())
            AnsiConsole.MarkupLine("[bold red]No[/] profiles found");
        return ExitCode.Success.ToInt();
    }
}
