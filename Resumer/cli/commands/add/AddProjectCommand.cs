using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

internal sealed class AddProjectCommand : Command<AddProjectSettings>
{
    public override int Execute(CommandContext context, AddProjectSettings settings)
    {
        var projectName = AnsiConsole.Ask<string>("Project Name:");
        var projectType = AnsiConsole.Prompt(new SimplePrompt<string>("Project Type:"));
        var projectDescription = AnsiConsole.Prompt(new SimplePrompt<string>("Project Description:"));
        var projectDetails = new List<string>();
        projectDetails.AddFromPrompt("Project Details (point form):");
        var projectUrl = AnsiConsole.Prompt(new SimplePrompt<string>("Project URL:"));
        var projectStartDate = AnsiConsole.Prompt(new SimplePrompt<DateOnly?>("Start Date:"));
        var projectEndDate = AnsiConsole.Prompt(new SimplePrompt<DateOnly?>("End Date:"));

        var project = new Project
        {
            Title = projectName,
            Type = projectType,
            Description = projectDescription,
            Details = projectDetails,
            Link = string.IsNullOrWhiteSpace(projectUrl) ? null : new Uri(projectUrl),
            StartDate = projectStartDate,
            EndDate = projectEndDate
        };

        var db = new ResumeContext();
        db.Projects.Add(project);
        db.SaveChanges();
        return CommandOutput.Success($"[green]Added project {projectName}[/]");
    }
}

public class AddProjectSettings : AddCommandSettings;