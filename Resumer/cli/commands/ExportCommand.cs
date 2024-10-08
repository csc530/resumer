using System.ComponentModel;
using System.Text;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands;

public class ExportCommand: Command<ExportCommandSettings>
{
    public override int Execute(CommandContext context, ExportCommandSettings settings)
    {
        var db = new ResumeContext();
        var dbProfiles = db.Profiles;

        if(!dbProfiles.Any())
            return CommandOutput.Error(ExitCode.NoData, "No profiles found",
                "Please add a profile before exporting (resumer add profile)");

        var name = AnsiConsole.Ask<string>("Resume title:");

        List<Job> jobs = [];
        List<Skill> skills = [];
        List<Project> projects = [];
        List<Education> education = [];

        var formatPrompt = new SelectionPrompt<Formats>()
            .Title("Select export format")
            .AddChoiceGroup(Formats.Text, Utility.TextFormats)
            .AddChoiceGroup(Formats.Binary, Utility.BinaryFormats)
            .UseConverter(format => format.ToString().ToLower())
            .EnableSearch();
        var format = AnsiConsole.Prompt(formatPrompt);


        var exportToFile = format.HasFlag(Formats.Binary) || AnsiConsole.Confirm("Export to file?");

        var profile = AnsiConsole.Prompt(new SelectionPrompt<Profile>()
            .Title("Select profile")
            .AddChoices(dbProfiles.AsEnumerable().OrderBy(profile => profile.WholeName))
            .WrapAround()
        );

        if(!db.Jobs.Any())
            CommandOutput.Warn("No jobs found");
        else
            jobs = AnsiConsole.Prompt(new MultiSelectionPrompt<Job>()
                .Title("Select jobs")
                .AddChoices(db.Jobs.OrderByDescending(job => job.StartDate))
                .WrapAround()
                .NotRequired()
            );

        // ? select specific job details
        if(jobs.Count != 0 && AnsiConsole.Confirm("Select specific job details?"))
            foreach(var job in jobs)
                if(job.Description.Count != 0)
                    job.Description =
                        AnsiConsole.Prompt(new MultiSelectionPrompt<string>().AddChoices(job.Description));
                else
                    CommandOutput.Warn($"No specific job descriptions found for \"{job.ToString().EscapeMarkup()}\"");

        if(!db.Skills.Any())
            CommandOutput.Warn("No skills found");
        else
            skills = AnsiConsole.Prompt(new MultiSelectionPrompt<Skill>()
                .Title("Select skills")
                .AddChoices(db.Skills)
                .WrapAround()
                .NotRequired()
            );

        if(!db.Education.Any())
            CommandOutput.Warn("No education found");
        else
            education = AnsiConsole.Prompt(new MultiSelectionPrompt<Education>()
                .Title("Select education")
                .AddChoices(db.Education)
                .WrapAround()
                .NotRequired()
            );

        if(!db.Projects.Any())
            CommandOutput.Warn("No projects found");
        else
            projects = AnsiConsole.Prompt(new MultiSelectionPrompt<Project>()
                .Title("Select projects")
                .AddChoices(db.Projects)
                .WrapAround()
                .NotRequired()
            );

        var template = TypstTemplate.Default;

        if(format.IsTypstFormat() && db.Templates.Any())
            template = AnsiConsole.Prompt(new SelectionPrompt<TypstTemplate>()
                .Title("Select template")
                .AddChoices(db.Templates)
                .WrapAround()
            );

        var resume = new Resume(name)
        {
            Profile = profile,
            Jobs = jobs,
            Skills = skills,
            Projects = projects,
            Education = education,
        };

        var bytes = format switch
        {
            Formats.Txt => Encoding.UTF8.GetBytes(resume.ExportToTxt()),
            Formats.Md => Encoding.UTF8.GetBytes(resume.ExportToMarkdown()),
            Formats.Json => Encoding.UTF8.GetBytes(resume.ExportToJson()),
            Formats.Typ => Encoding.UTF8.GetBytes(resume.ExportToTypst(template)),
            Formats.Svg => Encoding.UTF8.GetBytes(resume.ExportToSvg(template)),

            Formats.Pdf => resume.ExportToPdf(template),
            Formats.Png => resume.ExportToPng(template),
            _ => throw new NotSupportedException("Unsupported format"),
        };

        if(exportToFile)
        {
            var defaultFileName =
                $"{resume.Name}_{resume.DateCreated:yyyy-MM-dd_HH-mm-ss}.{format.ToString().ToLower()}";
            var fileName = AnsiConsole.Prompt(new TextPrompt<string>("file name:").DefaultValue(defaultFileName));

            File.WriteAllBytes(fileName, bytes);
            return CommandOutput.Success($"Exported to [bold]{fileName}[/]");
        }

        var output = Encoding.UTF8.GetString(bytes);
        return settings.Raw
            ? CommandOutput.Success(output.EscapeMarkup())
            : CommandOutput.Success(format == Formats.Json ? new JsonText(output) : new Text(output));
    }
}

public class ExportCommandSettings: CommandSettings
{
    [CommandOption("-r|--raw")]
    [Description("Raw output")]
    public bool Raw { get; set; }
}