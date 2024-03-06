using System.ComponentModel;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.export;

public class ExportCommand: Command<ExportCommandSettings>
{
    public override int Execute(CommandContext context, ExportCommandSettings settings)
    {
        var db = new ResumeContext();
        if(!db.Profiles.Any())
            return Globals.CommandError("No profiles found",ExitCode.NoData, "Please add a profile before exporting, use the 'add profile' command");

        var format = settings.Format;
        var output = settings.Output;
        var template = settings.Template;
        List<Job> jobs = [];
        List<Skill> skills = [];
        List<Project> projects = [];
        List<string> languages = [];
        List<Education> education = [];
        Profile profile;
        List<Certificate> certifications = [];

        var formatPrompt = new SelectionPrompt<string>().Title("Select export format")
                                                        .AddChoiceGroup("plain text", Globals.TextFormatNames)
                                                        .AddChoiceGroup("binary", Globals.BinaryFormatNames);
        var outputPrompt = new TextPrompt<string>("Output file (null will write to stdout)").AllowEmpty()
           .DefaultValue($"resume.{format.ToLower()}");
        var profilePrompt = new SelectionPrompt<Profile>().Title("Select profile").AddChoices(db.Profiles);
        var jobPrompt = new MultiSelectionPrompt<Job>().Title("Select jobs").AddChoices(db.Jobs).NotRequired();
        var skillPrompt = new MultiSelectionPrompt<Skill>().Title("Select skills").AddChoices(db.Skills).NotRequired();
        var projectPrompt = new MultiSelectionPrompt<Project>().Title("Select projects").AddChoices(db.Projects).NotRequired();

        format = AnsiConsole.Prompt(formatPrompt);
        output = AnsiConsole.Prompt(outputPrompt);
        profile = AnsiConsole.Prompt(profilePrompt);

        if(!db.Jobs.Any())
            AnsiConsole.MarkupLine("[yellow]No jobs found[/]");
        else
            jobs = AnsiConsole.Prompt(jobPrompt);
        if(!db.Skills.Any())
            AnsiConsole.MarkupLine("[yellow]No skills found[/]");
        else
            skills = AnsiConsole.Prompt(skillPrompt);
        if(!db.Projects.Any())
            AnsiConsole.MarkupLine("[yellow]No projects found[/]");
        else
            projects = AnsiConsole.Prompt(projectPrompt);

        //? dependent upon profile
        var certificatePrompt = new MultiSelectionPrompt<Certificate>().Title("Select certificates")
                                                                       .AddChoices(profile.Certifications)
                                                                       .NotRequired();
        var languagePrompt = new MultiSelectionPrompt<string>().Title("Select languages")
                                                               .AddChoices(profile.Languages).NotRequired();
        var educationPrompt = new MultiSelectionPrompt<Education>().Title("Select education")
                                                                   .AddChoices(profile.Education).NotRequired();

        if(profile.Certifications.Count == 0)
            AnsiConsole.MarkupLine("[yellow]No certifications found[/]");
        else
            certifications = AnsiConsole.Prompt(certificatePrompt);
        if(profile.Languages.Count == 0)
            AnsiConsole.MarkupLine("[yellow]No languages found[/]");
        else
            languages = AnsiConsole.Prompt(languagePrompt);
        if(profile.Education.Count == 0)
            AnsiConsole.MarkupLine("[yellow]No education found[/]");
        else
            education = AnsiConsole.Prompt(educationPrompt);


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


        return ExitCode.Success.ToInt();
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