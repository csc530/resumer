using System.Diagnostics.CodeAnalysis;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

internal sealed class AddProjectCommand : Command<AddProjectSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] AddProjectSettings settings)
    {
        var projectName = AnsiConsole.Ask<string>("Project Name:");
        var projectType = AnsiConsole.Prompt(new SimplePrompt<string>("Project Type:"));
        var projectDescription = AnsiConsole.Prompt(new SimplePrompt<string>("Project Description:"));
        var projectDetails = new List<string>();
        projectDetails.AddFromPrompt("Project Details (point form):");
        var projectUrl = AnsiConsole.Prompt(new TextPrompt<Uri?>("Project URL:").HideDefaultValue().AllowEmpty());
        var projectStartDate = AnsiConsole.Prompt(new SimplePrompt<DateOnly?>("Start Date:"));
        var projectEndDate = AnsiConsole.Prompt(new SimplePrompt<DateOnly?>("End Date:"));

        var project = new Project()
        {
            Name = projectName,
            Type = projectType,
            Description = projectDescription,
            Details = projectDetails,
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

public class AddProjectSettings : AddCommandSettings;