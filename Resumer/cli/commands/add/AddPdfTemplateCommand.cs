using System.ComponentModel;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public class AddPdfTemplateCommand: Command<AddPdfTemplateCommandSettings>
{
    public override int Execute(CommandContext context, AddPdfTemplateCommandSettings settings)
    {
        var templatePath = settings.File;
        Uri? uri = null;
        if(!Path.Exists(templatePath) && !Uri.TryCreate(templatePath, UriKind.Absolute, out uri))
            return CommandOutput.Error(ExitCode.InvalidArgument, "file not found");

        var templateContent = uri == null
            ? File.ReadAllText(templatePath)
            : new HttpClient().GetStringAsync(uri).Result;
        var template = new TypstTemplate(Path.GetFileNameWithoutExtension(templatePath), templateContent);


        if(template.isValid(out var error, out var output))
        {
            if(settings.Verbose)
            {
                if(!string.IsNullOrWhiteSpace(output))
                    AnsiConsole.WriteLine(output);
                var lines = templateContent.Split("\n").Length;
                CommandOutput.Verbose($"template file", templatePath);
                CommandOutput.Verbose($"lines", lines.ToString());
            }

            template.Name = AnsiConsole.Prompt(Utility.SimplePrompt("template name:", template.Name));
            template.Description = AnsiConsole
                .Prompt(Utility.SimplePrompt("template description:", template.Description));
            var db = new ResumeContext();
            db.Templates.Add(template);
            db.SaveChanges();
            return CommandOutput.Success($"Template [bold]{template.Name}[/] added");
        }

        if(settings.Verbose && !string.IsNullOrWhiteSpace(error))
            AnsiConsole.WriteLine(error);

        return CommandOutput.Error(ExitCode.Fail, "invalid template file", "please check the template files typst syntax (test against example template file with 'resumer generate')");
    }
}

public class AddPdfTemplateCommandSettings: CommandSettings
{
    [CommandArgument(0, "<PATH|URL>")]
    [Description("typst resume template file")]
    public string File { get; set; }

    [CommandOption("-V|--verbose")]
    [Description("Verbose output")]
    public bool Verbose { get; set; }
}