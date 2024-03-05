using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public class AddProjectSettings: AddCommandSettings
{
    public bool PromptUser => (ProjectName == null && ProjectDescription == null && ProjectType == null && ProjectUrl == null && ProjectStartDate == null && ProjectEndDate == null) || Interactive;

    [CommandOption("-n|--name <NAME>")]
    [Description("The name of the project")]

    public string? ProjectName { get; set; }

    [CommandOption("-t|--type <TYPE>")]
    [Description("The type of the project")] //todo: enum?[]
    public string? ProjectType { get; set; }

    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the project")]
    public string? ProjectDescription { get; set; }

    [CommandOption("-u|--url <URL>")]
    [Description("The url to the project")]
    public Uri? ProjectUrl { get; set; }

    [CommandOption("-s|--start <START>")]
    [Description("The start date of the project")]
    public DateOnly? ProjectStartDate { get; set; }

    [CommandOption("-e|--end <END>")]
    [Description("The end date of the project")]
    public DateOnly? ProjectEndDate { get; set; }
}

internal sealed class AddProjectCommand: Command<AddProjectSettings>
{
    public override int Execute(CommandContext context, AddProjectSettings settings)
    {
        var projectName = settings.ProjectName;
        var projectType = settings.ProjectType;
        var projectDescription = settings.ProjectDescription;
        var projectUrl = settings.ProjectUrl;
        var projectStartDate = settings.ProjectStartDate;
        var projectEndDate = settings.ProjectEndDate;
        Project? project = null;
        if(settings.PromptUser)
        {
            projectName = AnsiConsole.Ask<string>("Project Name:");
            projectType = RenderableFactory.CreateTextPrompt<string?>("Project Type:", allowEmpty: true).Show();
            projectDescription = RenderableFactory.CreateTextPrompt<string?>("Project Description:", allowEmpty: true).Show();
            projectUrl = new TextPrompt<Uri?>("Project URL:").AllowEmpty().ShowDefaultValue().Show();
            projectStartDate = RenderableFactory.CreateTextPrompt<DateOnly?>("Start Date:", allowEmpty: true).Show();
            projectEndDate = RenderableFactory.CreateTextPrompt<DateOnly?>("End Date:", allowEmpty: true).Show();
        }

        project = new Project(projectName!)
        {
            Type = projectType,
            Description = projectDescription,
            Link = projectUrl,
            StartDate = projectStartDate,
            EndDate = projectEndDate
        };

        var db = new ResumeContext();
        db.Projects.Add(project);
        db.SaveChanges();
        AnsiConsole.MarkupLine($"[green]Added project {projectName}[/]");
        return ExitCode.Success.ToInt();
    }
}