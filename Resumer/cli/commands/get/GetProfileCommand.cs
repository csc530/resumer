using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.get;

public class GetProfileCommand: Command<GetProfileCommandSettings>
{
    public override int Execute(CommandContext context, GetProfileCommandSettings settings)
    {
        var profiles = new ResumeContext().Profiles.ToList();

        if(profiles.Count == 0)
            return CommandOutput.Success("[bold red]No[/] profiles found");
        else
        {
            var table = settings.CreateTable();
            if(table == null)
                profiles.ForEach(profile => AnsiConsole.WriteLine(profile.ToString()));
            else
            {
                profiles.ForEach(profile => settings.AddProfileToTable(table, profile));
                AnsiConsole.Write(table);
            }
            return CommandOutput.Success();
        }
    }
}

public class GetProfileCommandSettings: OutputCommandSettings
{
    public  Table? CreateTable()
    {
        var table = this.CreateTable("Profile");
        if(table == null) return table;

        table.AddColumns("Name", "Email", "Phone", "Location", "Interests", "Objective", "Languages", "Website");
        table.Title = new TableTitle("Profiles");

        return table;
    }

    public void AddProfileToTable(Table table, Profile profile)
    {
        table.AddRow(
            profile.WholeName,
            profile.EmailAddress,
            profile.PhoneNumber,
            profile.Location.Print(),
            profile.Interests.Print(),
            profile.Objective.Print(),
            profile.Languages.Print(),
            profile.Website.Print());
    }
}