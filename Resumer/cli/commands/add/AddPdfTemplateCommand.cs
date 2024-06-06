using System.ComponentModel;
using Resumer.models;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public class AddPdfTemplateCommand: Command<AddPdfTemplateCommandSettings>
{
    public override int Execute(CommandContext context, AddPdfTemplateCommandSettings settings)
    {
        var templatePath = settings.File;
        if(!Path.Exists(templatePath))
            return CommandOutput.Error(ExitCode.InvalidArgument, "file not found");
        var template = File.ReadAllText(templatePath);

        if(TestTemplate(template))
            return CommandOutput.Error(ExitCode.MissingArgument, "Missing argument", "Please provide a file to add");
        return CommandOutput.Success("Not implemented");
    }

    private bool TestTemplate(string template)
    {
        var testResume = Resume.ExampleResume();
        testResume.ExportToPdf(template);
        return true;
    }
}

public class AddPdfTemplateCommandSettings: CommandSettings
{
    [CommandArgument(0, "<FILE>")]
    [Description("Typst resume template file")]
    public string File { get; set; }
}