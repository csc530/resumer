using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.export;

public class ExportCommand: Command<ExportCommandSettings>
{
    public override int Execute(CommandContext context, ExportCommandSettings settings)
    {
        var format = settings.Format;
        var output = settings.Output;
        var template = settings.Template;

        var formatPrompt = new SelectionPrompt<string>().Title("Select export format")
                                                        .AddChoices(Enum.GetNames<Formats>());
        var outputPrompt = new TextPrompt<string>("Output file (null will write to stdout)").AllowEmpty();



        Console.WriteLine($"Exporting to {format} format");
        Console.WriteLine($"Output file: {output}");
        Console.WriteLine($"Template file: {template}");

        return 0;
    }
}

public class ExportCommandSettings: CommandSettings
{
    [CommandOption("-f|--format <FORMAT>")]
    [Description("Export format")]
    [DefaultValue("txt")]
    public string Format { get; set; }

    [CommandOption("-o|--output <OUTPUT>")]
    [Description("Output file")]
    public string Output { get; set; }

    [CommandOption("-t|--template <TEMPLATE>")]
    [Description("Template file")]
    public string Template { get; set; }
}