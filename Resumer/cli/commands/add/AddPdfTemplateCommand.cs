using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using Octokit;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public class AddPdfTemplateCommand: Command<AddPdfTemplateCommandSettings>
{
    public override int Execute(CommandContext context, AddPdfTemplateCommandSettings settings)
    {
        using var db = new ResumeContext();
        var dbTemplates = db.Templates;
        if(settings.All)
        {
            var version = Assembly.GetAssembly(typeof(Program))?.GetName().Version?.ToString();
            var ghClient = new GitHubClient(new ProductHeaderValue("resumer", version));
            var templates = ghClient.Repository.Content.GetAllContents("csc530", "Resumer", "templates")
                .Result
                .Select(x => ghClient.Repository.Content.GetAllContents("csc530", "Resumer", x.Path).Result)
                .SelectMany(x => x)
                .Select(x =>
                {
                    var template = new TypstTemplate(Path.GetFileNameWithoutExtension(x.Name), x.Content);
                    if(x.Content.StartsWith("//"))
                        template.Description = x.Content.Split("\n")[0][2..].Trim();
                    return template;
                })
                .ToImmutableArray();

            CommandOutput.Verbose("found", $"{templates.Length} templates", settings.Verbose);

            if(AnsiConsole.Confirm($"add {templates.Length} templates?"))
            {
                //search for templates with conflicting keys
                var conflicts = templates
                    .Where(x => dbTemplates.Any(y => y.Name == x.Name))
                    .ToImmutableArray();

                if(conflicts.Length > 0)
                {
                    CommandOutput.Warn($"{conflicts.Length} conflicting templates found");
                    CommandOutput.Warn(conflicts.Print().EscapeMarkup());
                    if(settings.Force || AnsiConsole.Confirm($"overwrite {conflicts.Length} conflicting templates?"))
                    {
                        CommandOutput.Verbose("updating conflicts", string.Join(", ", conflicts.Select(x => x.Name)),
                            settings.Verbose);
                        dbTemplates.UpdateRange(conflicts);
                    }
                    else
                        CommandOutput.Verbose("skipping", string.Join(", ", conflicts.Select(x => x.Name)),
                            settings.Verbose);
                }

                templates = [..templates.Except(conflicts)];

                CommandOutput.Verbose("adding", string.Join(", ", templates.Select(x => x.Name)), settings.Verbose);
                dbTemplates.AddRange(templates);
                var changes = db.SaveChanges();
                return CommandOutput.Success($"{changes} templates successfully added");
            }
            else
                return CommandOutput.Error(ExitCode.Canceled, "aborted");
        }
        else if(settings.File != null)
        {
            var templatePath = settings.File;
            Uri? uri = null;
            if(!Path.Exists(templatePath) && !Uri.TryCreate(templatePath, UriKind.Absolute, out uri))
                return CommandOutput.Error(ExitCode.InvalidArgument, "file not found");

            var templateContent = Path.Exists(templatePath)
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
                template.Description =
                    AnsiConsole.Prompt(Utility.SimplePrompt("template description:", template.Description));

                var conflictingTemplate = db.Templates.FirstOrDefault(x => x.Name == template.Name);
                if(conflictingTemplate != null)
                {
                    if(!settings.Force && !AnsiConsole.Confirm($"overwrite template [bold]{template.Name}[/]?"))
                        return CommandOutput.Error(ExitCode.Canceled, "aborted");
                    db.Templates.Remove(conflictingTemplate);
                }

                dbTemplates.Add(template);
                db.SaveChanges();
                return CommandOutput.Success($"Template [bold]{template.Name}[/] added");
            }

            if(settings.Verbose && !string.IsNullOrWhiteSpace(error))
                AnsiConsole.WriteLine(error);

            return CommandOutput.Error(ExitCode.Fail, "invalid template file",
                "please check the template files typst syntax (test against example template file with 'resumer generate')");
        }

        return CommandOutput.Error(ExitCode.MissingArgument, "missing template file path");
    }
}

public class AddPdfTemplateCommandSettings: CommandSettings
{
    [CommandArgument(0, "[PATH|URL]")]
    [Description("typst resume template file")]
    public string? File { get; set; }

    [CommandOption("-V|--verbose")]
    [Description("verbose output")]
    public bool Verbose { get; set; }

    [CommandOption("-a|--all")]
    [Description("add all templates from online repository")]
    public bool All { get; set; }

    [CommandOption("-f|--force")]
    [Description("overwrite conflicting templates")]
    public bool Force { get; set; }
}