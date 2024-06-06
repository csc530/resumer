using System.ComponentModel;
using System.Text;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Command = Spectre.Console.Cli.Command;

namespace Resumer.cli.commands;

public class GenerateExampleTypstTemplate: Command<GenerateExampleTypstTemplateSettings>
{
    public override int Execute(CommandContext context, GenerateExampleTypstTemplateSettings settings)
    {
        var path = $"{settings.Name}.typ";
        if(!settings.Raw && !settings.Force && File.Exists(path))
            return CommandOutput.Error(ExitCode.Fail, $"file already exists - [italic]{path}[/]");

        var resume = Resume.ExampleResume();
        var template = new StringBuilder();
        template.Append("// generated on ");
        template.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        template.AppendLine("// by [bold]Resumer[/]");
        template.AppendLine();
        template.AppendLine("// example resume information");
        template.AppendLine("// used to illustrate injected variables names and structure");
        template.AppendLine(Utility.PrintAsTypstVariables(resume));
        template.AppendLine(new ResumeContext().Templates.First(tmp => tmp.Id == Guid.Empty).Content);

        if(settings.Raw)
            return CommandOutput.Success(template.ToString().EscapeMarkup());
        else
            File.WriteAllText(path, template.ToString());
        return CommandOutput.Success();
    }
}

public class GenerateExampleTypstTemplateSettings: CommandSettings
{
    [CommandOption("-n|--name")]
    [Description("Name of the template file to be created")]
    public string Name { get; set; } = "template";

    [CommandOption("-f|--force")]
    [Description("Overwrite existing file")]
    public bool Force { get; set; }

    [CommandOption("--raw|-r")]
    [Description("Print template to stdout")]
    public bool Raw { get; set; }

    public override ValidationResult Validate() => string.IsNullOrEmpty(Name)
        ? ValidationResult.Error("Name is required")
        : ValidationResult.Success();
}