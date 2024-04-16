using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.export;

public class ExportCommand: Command<ExportCommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ExportCommandSettings settings)
    {
        var db = new ResumeContext();
        if(!db.Profiles.Any())
            return CommandOutput.Error(ExitCode.NoData, "No profiles found",
                "Please add a profile before exporting, use the 'add profile' command");

        string format;
        string output;
        var template = settings.Template;
        List<Job> jobs = [];
        List<Skill> skills = [];
        List<Project> projects = [];
        List<string> languages = [];
        List<Education> education = [];
        Profile profile;
        List<Certificate> certifications = [];

        var formatPrompt = new SelectionPrompt<string>().Title("Select export format").AddChoiceGroup("plain text", Globals.TextFormatNames).AddChoiceGroup("binary", Globals.BinaryFormatNames);

        format = AnsiConsole.Prompt(formatPrompt);
        output = AnsiConsole.Prompt(new TextPrompt<string>("Output file (no input will write to stdout)").AllowEmpty());
        profile = AnsiConsole.Prompt(new SelectionPrompt<Profile>().Title("Select profile").AddChoices(db.Profiles));

        if(!db.Jobs.Any())
            AnsiConsole.MarkupLine("[yellow]No jobs found[/]");
        else
            jobs = AnsiConsole.Prompt(new MultiSelectionPrompt<Job>().Title("Select jobs").AddChoices(db.Jobs).NotRequired());

        if(!db.Skills.Any())
            AnsiConsole.MarkupLine("[yellow]No skills found[/]");
        else
            skills = AnsiConsole.Prompt(new MultiSelectionPrompt<Skill>().Title("Select skills").AddChoices(db.Skills).NotRequired());

        if(!db.Projects.Any())
            AnsiConsole.MarkupLine("[yellow]No projects found[/]");
        else
            projects = AnsiConsole.Prompt(new MultiSelectionPrompt<Project>().Title("Select projects").AddChoices(db.Projects).NotRequired());

        //? dependent upon profile
        if(profile.Certifications.Count == 0)
            AnsiConsole.MarkupLine("[yellow]No certifications found[/]");
        else
            certifications = AnsiConsole.Prompt(new MultiSelectionPrompt<Certificate>().Title("Select certificates").AddChoices(profile.Certifications).NotRequired());
        if(profile.Languages.Count == 0)
            AnsiConsole.MarkupLine("[yellow]No languages found[/]");
        else
            languages = AnsiConsole.Prompt(new MultiSelectionPrompt<string>().Title("Select languages").AddChoices(profile.Languages).NotRequired());
        if(profile.Education.Count == 0)
            AnsiConsole.MarkupLine("[yellow]No education found[/]");
        else
            education = AnsiConsole.Prompt(new MultiSelectionPrompt<Education>().Title("Select education").AddChoices(profile.Education).NotRequired());


        Console.WriteLine($"Exporting to {format} format");
        Console.WriteLine($"Output file: {output}");
        Console.WriteLine($"Template file: {template}");
        Console.WriteLine($"Profile: {profile}");
        Console.WriteLine($"Jobs: {jobs}");
        Console.WriteLine($"Skills: {skills}");
        Console.WriteLine($"Projects: {projects}");
        Console.WriteLine($"Languages: {languages}");
        Console.WriteLine($"Education: {education}");
        Console.WriteLine($"Certifications: {certifications}");


        return CommandOutput.Success();
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