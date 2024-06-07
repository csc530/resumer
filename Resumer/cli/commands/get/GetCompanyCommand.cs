using Microsoft.EntityFrameworkCore;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetCompanyCommand : Command<GetCompanyCommandSettings>
{
    public override int Execute(CommandContext context, GetCompanyCommandSettings settings)
    {
        ResumeContext database = new();
        var companies = database.Jobs.Select(job => job.Company).Distinct();

        if(!companies.Any())
            AnsiConsole.MarkupLine("No companies found");
        else
        {
            var table = settings.CreateTable("Companies");
            if(table == null)
                companies.ForEachAsync(AnsiConsole.WriteLine).Wait();
            else
            {
                table.AddColumn("Name");
                foreach(var company in companies)
                    table.AddRow(company);
                AnsiConsole.Write(table);
            }
        }

        return CommandOutput.Success();
    }
}

public class GetCompanyCommandSettings : OutputCommandSettings;