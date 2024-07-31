using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetCompanyCommand: Command<GetCompanyCommandSettings>
{
    public override int Execute(CommandContext context, GetCompanyCommandSettings settings)
    {
        ResumeContext database = new();
        var companies = database.Jobs.Select(job => job.Company).Distinct().ToList();

        if(companies.Count == 0)
            AnsiConsole.MarkupLine("No companies found");
        else
        {
            var table = settings.CreateTable("Companies");
            if(table == null)
                companies.ForEach(AnsiConsole.WriteLine);
            else
            {
                table.AddColumn("Name").HideHeaders();
                companies.ForEach(company => table.AddRow(company));
                AnsiConsole.Write(table);
            }
        }

        return CommandOutput.Success();
    }
}

public class GetCompanyCommandSettings: OutputCommandSettings;