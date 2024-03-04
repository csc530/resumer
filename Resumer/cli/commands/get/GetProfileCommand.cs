using System.Diagnostics.CodeAnalysis;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetProfileCommand: Command<GetProfileCommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] GetProfileCommandSettings settings)
    {
        var profiles = new ResumeContext().Profiles;
        var rows = new Table();
        foreach(var profile in profiles)
            rows.AddRow(profile.WholeName, profile.EmailAddress, profile.PhoneNumber, profile.Summary.GetPrintValue(),
                profile.Website.GetPrintValue());
        AnsiConsole.Write(rows);
        settings.GetTable();
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLineInterpolated($"[bold green]{profiles.Count()}[/] profiles found");
        return ExitCode.Success.ToInt();
    }
}

public class GetProfileCommandSettings: OutputCommandSettings
{
}