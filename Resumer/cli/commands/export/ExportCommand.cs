using System.ComponentModel;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.export;

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

        var formatPrompt = new SelectionPrompt<Formats>()
            .Title("Select export format")
            .AddChoiceGroup(Formats.Text, Utility.TextFormats)
            .AddChoiceGroup(Formats.Binary, Utility.BinaryFormats)
            .WrapAround();

        var format = AnsiConsole.Prompt(formatPrompt);
        var exportToFile = AnsiConsole.Confirm("Export to file?");

        var profile = AnsiConsole.Prompt(new SelectionPrompt<Profile>()
            .Title("Select profile")
            .AddChoices(dbProfiles.AsEnumerable().OrderBy(profile => profile.WholeName)));

        if(!db.Jobs.Any())
            CommandOutput.Warn("No jobs found");
        else
            jobs = AnsiConsole.Prompt(new MultiSelectionPrompt<Job>()
                .Title("Select jobs")
                .AddChoices(db.Jobs.OrderByDescending(job => job.StartDate))
                .NotRequired());

        if(!db.Skills.Any())
            CommandOutput.Warn("No skills found");
        else
            skills = AnsiConsole.Prompt(new MultiSelectionPrompt<Skill>()
                .Title("Select skills")
                .AddChoices(db.Skills)
                .NotRequired());

        if(!db.Projects.Any())
            CommandOutput.Warn("No projects found");
        else
            projects = AnsiConsole.Prompt(new MultiSelectionPrompt<Project>()
                .Title("Select projects")
                .AddChoices(db.Projects)
                .NotRequired());


        var resume = new Resume
        {
            Profile = profile,
            Jobs = jobs,
            Skills = skills,
            Projects = projects,
            Name = name,
        };


        var output = format switch
        {
            Formats.Txt => resume.ExportToTxt(),
            Formats.Md => resume.ExportToMarkdown(),
            Formats.Json => resume.ExportToJson(),
            // Formats.Binary => resume.ExportToBinary(),
            _ => throw new NotSupportedException("Unsupported format"),
        };

        if(exportToFile)
        {
            var defaultFileName =
                $"{resume.Name}_{resume.DateCreated:yyyy-MM-dd_HH-mm-ss}.{format.ToString().ToLower()}";
            var fileName = AnsiConsole.Prompt(new TextPrompt<string>("file name:").DefaultValue(defaultFileName));

            File.WriteAllText(fileName, output);
            return CommandOutput.Success($"Exported to [bold]{fileName}[/]");
        }
        else if(settings.Raw)
            return CommandOutput.Success(output.EscapeMarkup());
        else
            return CommandOutput.Success(format == Formats.Json ? new JsonText(output) : new Text(output));
    }
}

public class ExportCommandSettings: CommandSettings
{
    [CommandOption("-f|--format <FORMAT>")]
    [Description("Export format")]
    [DefaultValue("txt")]
    public required string? Format { get; set; }

    [CommandOption("-o|--output <OUTPUT>")]
    [Description("Output file")]
    public required string? Output { get; set; }

    [CommandOption("-t|--template <TEMPLATE>")]
    [Description("Template file")]
    public required string? Template { get; set; }

    [CommandOption("-r|--raw")]
    [Description("Raw output")]
    public bool Raw { get; set; }
}