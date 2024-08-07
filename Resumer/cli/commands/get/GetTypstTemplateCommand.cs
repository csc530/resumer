using System.ComponentModel;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetTypstTemplateCommand: Command<GetTypstTemplateCommandSettings>
{
    public override int Execute(CommandContext context, GetTypstTemplateCommandSettings settings)
    {
        var templates = new ResumeContext().Templates.ToList();

        if(templates.Count == 0)
            return CommandOutput.Success("no templates found");

        if(settings.Name != null)
        {
            var template = templates.Find(t => t.Name.Equals(settings.Name, StringComparison.OrdinalIgnoreCase));
            return template == null
                ? CommandOutput.Error(ExitCode.NotFound, $"template [bold]{settings.Name}[/] not found")
                : CommandOutput.Success(template.Content.EscapeMarkup());
        }

        var table = settings.CreateTable();
        if(table == null)
            return PlainPrintTemplates(settings, templates);
        templates.ForEach(template => settings.AddTypstTemplateToTable(table, template));

        return CommandOutput.Success(table);
    }

    private static int PlainPrintTemplates(GetTypstTemplateCommandSettings settings, List<TypstTemplate> templates)
    {
        if(settings.Full)
            templates.ForEach(template =>
            {
                AnsiConsole.MarkupLineInterpolated($"[bold]{template}[/]");
                AnsiConsole.WriteLine(Utility.DashSeparator);
                AnsiConsole.WriteLine(template.Content);
                AnsiConsole.WriteLine();
            });
        else
            templates.ForEach(template => AnsiConsole.WriteLine(template.ToString()));
        return CommandOutput.Success();
    }
}

public class GetTypstTemplateCommandSettings: OutputCommandSettings
{
    /// <summary>
    /// show full typst template content
    /// </summary>
    [CommandOption("-f|--full")]
    [Description("show full typst template content")]
    public bool Full { get; init; }

    /// <summary>
    /// Name of the template to get
    /// </summary>
    [CommandArgument(0, "[name]")]
    [Description("template name")]
    public string? Name { get; init; }

    public override Table? CreateTable()
    {
        var table = base.CreateTable("Templates");
        if(table == null) return table;
        table.AddColumns("Name", "Description");
        if(Full)
            table.AddColumn("Content");
        return table;
    }

    public void AddTypstTemplateToTable(Table table, TypstTemplate template)
    {
        if(Full)
            table.AddRow(
                template.Name.EscapeMarkup(),
                template.Description.EscapeMarkup(),
                template.Content.EscapeMarkup());
        else
            table.AddRow(template.Name.EscapeMarkup(), template.Description.EscapeMarkup());
    }
}