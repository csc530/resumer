using System.Diagnostics.CodeAnalysis;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetCompanyCommand : Command<OutputCommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] OutputCommandSettings settings)
    {
        ResumeContext database = new();
        var companies = database.Jobs.Select(job => job.Company).Distinct();

        if(!companies.Any())
            AnsiConsole.MarkupLine("No companies found");
        else
        {
            var table = settings.GetTable();
            if(table == null)
                foreach(var company in companies)
                    AnsiConsole.WriteLine(company);
            else
            {
                table.AddTableColumn("Company Name");
                foreach(var company in companies)
                    table.AddRow(company);
                AnsiConsole.Write(table);
            }
        }

        return CommandOutput.Success();
    }
}