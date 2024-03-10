using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public class AddProjectSettings: AddCommandSettings
{
}

internal sealed class AddProjectCommand: Command<AddProjectSettings>
{
    public override int Execute(CommandContext context, AddProjectSettings settings)
    {
        var projectName = AnsiConsole.Ask<string>("Project Name:");
        var projectType = RenderableFactory.CreateTextPrompt<string?>("Project Type:", allowEmpty: true).Show();
        var projectDescription = RenderableFactory.CreateTextPrompt<string?>("Project Description:", allowEmpty: true).Show();
        var projectUrl = new TextPrompt<Uri?>("Project URL:").AllowEmpty().ShowDefaultValue().Show();
        var projectStartDate = RenderableFactory.CreateTextPrompt<DateOnly?>("Start Date:", allowEmpty: true).Show();
        var projectEndDate = RenderableFactory.CreateTextPrompt<DateOnly?>("End Date:", allowEmpty: true).Show();


        var project = new Project(projectName!)
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
        return CommandOutput.Success($"[green]Added project {projectName}[/]");
    }
}