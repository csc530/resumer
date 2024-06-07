using System.ComponentModel;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetTypstTemplateCommand: Command<GetTypstTemplateCommandSettings>
{
    public override int Execute(CommandContext context, GetTypstTemplateCommandSettings settings)
    {
        var db = new ResumeContext();
        var templates = db.Templates.ToList();

        if(templates.Count == 0)
            return CommandOutput.Success("no templates found");

        if(settings.Name != null)
        {
            var template = templates.Find(t => t.Name.Equals(settings.Name, StringComparison.OrdinalIgnoreCase));
            if(template == null)
                return CommandOutput.Error(ExitCode.NotFound, $"template [bold]{settings.Name}[/] not found");
            else
            {
                //verbose tings
                // AnsiConsole.MarkupLineInterpolated($"[bold]{template.Name}[/]");
                // AnsiConsole.MarkupLine(template.Description.EscapeMarkup());
                return CommandOutput.Success(template.Content.EscapeMarkup());
            }
        }

        var table = settings.CreateTable<TypstTemplate>();
        if(!settings.Full)
        {
            table = new Table().AddColumn("Name").AddColumn("Description");
            templates.ForEach(t => table.AddRow(t.Name.EscapeMarkup(), t.Description.EscapeMarkup()));
            return CommandOutput.Success(table);
        }
        else
            return CommandOutput.Success(table);
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
}