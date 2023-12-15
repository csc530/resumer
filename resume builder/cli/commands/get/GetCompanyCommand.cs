using System.Diagnostics.CodeAnalysis;
using resume_builder.cli.settings;
using resume_builder.models;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get;

public class GetCompanyCommand : Command<OutputCommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] OutputCommandSettings settings)
    {
        ResumeContext database = new();
        var companies = database.Companies;
        if(companies.Count() == 0)
            AnsiConsole.MarkupLine("No companies found");
        else
        {
            var table = settings.GetTable();
            if(table == null)
                foreach(var company in companies)
                    AnsiConsole.WriteLine(company.Name);
            else
            {
                table.AddTableColumn("Company Name");
                foreach(var company in companies)
                    table.AddRow(company.Name);
                AnsiConsole.Write(table);
            }
        }

        return ExitCode.Success.ToInt();
    }
}