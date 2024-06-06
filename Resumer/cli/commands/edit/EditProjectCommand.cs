using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Command = Spectre.Console.Cli.Command;

namespace Resumer.cli.commands.edit;

public class EditProjectCommand: Command
{
    public override int Execute(CommandContext context)
    {
        var db = new ResumeContext();
        var projects = db.Projects.ToList();
        if(projects.Count == 0)
            return CommandOutput.Success("No projects to edit");
        var project = AnsiConsole.Prompt(new SelectionPrompt<Project>()
            .Title("Select a project to edit")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more projects)[/]")
            .AddChoices(projects)
        );

        var name = AnsiConsole.Prompt(new TextPrompt<string>("Enter new project name:").DefaultValue(project.Title));
        var type = AnsiConsole.Prompt(new TextPrompt<string?>("Enter new project type:").DefaultValue(project.Type));
        var description =AnsiConsole.Prompt(new TextPrompt<string?>("Enter new project description:").DefaultValue(project.Description));
        var startDate =AnsiConsole.Prompt(new TextPrompt<DateOnly?>("Enter new project start date:").DefaultValue(project.StartDate));
        var endDate = AnsiConsole.Prompt(new TextPrompt<DateOnly?>("Enter new project end date:").DefaultValue(project.EndDate));

        string? projectUrl;
        bool isValidUri = false;
        Uri? projectUri;
        do
        {
            projectUrl =
                AnsiConsole.Prompt(new TextPrompt<string?>("Enter new project URL: ").DefaultValue(project.Link?.ToString()));
            isValidUri = Uri.TryCreate(projectUrl, UriKind.Absolute, out projectUri);
            if(!isValidUri && projectUrl != null)
                AnsiConsole.MarkupLine("[red]Invalid URL. Please enter a valid URL.[/]");
        } while(!isValidUri && projectUrl != null);


        project.Title = name;
        project.Type = type;
        project.Description = description;
        project.StartDate = startDate;
        project.EndDate = endDate;
        project.Link = projectUri ?? project.Link;
        project.Details.EditFromPrompt("Edit project details (point form):");

        db.SaveChanges();

        return CommandOutput.Success("Project edited successfully");
    }
}