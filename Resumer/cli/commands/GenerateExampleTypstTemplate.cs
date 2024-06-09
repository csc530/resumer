using System.ComponentModel;
using System.Text;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands;

public class GenerateExampleTypstTemplate: Command<GenerateExampleTypstTemplateSettings>
{
    public override int Execute(CommandContext context, GenerateExampleTypstTemplateSettings settings)
    {
        var resume = Resume.ExampleResume();
        var template = new StringBuilder();
        template.Append("// generated on ");
        template.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        template.AppendLine("// by [bold]Resumer[/]");
        template.AppendLine();
        template.AppendLine("// example resume information");
        template.AppendLine("// used to illustrate injected variables names and structure");
        template.AppendLine(resume.ExportToTypst(TypstTemplate.Default,settings.Pretty));
        //* because ansi console adds newlines at terminal widths
        Console.WriteLine(template.ToString());
        return CommandOutput.Success();
    }
}

public class GenerateExampleTypstTemplateSettings: CommandSettings{
    [CommandOption("-p|--pretty")]
    [DefaultValue(false)]
    [Description("Prints the output in pretty format")]
    public bool Pretty { get; set; }

}