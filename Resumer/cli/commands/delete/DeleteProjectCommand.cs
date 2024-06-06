using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Command = Spectre.Console.Cli.Command;

namespace Resumer.cli.commands.delete;

public class DeleteProjectCommand: Command
{
    public override int Execute(CommandContext context)
    {
        var db = new ResumeContext();
        var projects = db.Projects;
        if(!projects.Any())
            return CommandOutput.Success("No projects to delete");
        var selectedProjects = AnsiConsole.Prompt(new MultiSelectionPrompt<Project>()
            .Title("Select a project to delete")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more projects)[/]")
            .AddChoices(projects)
        );
        db.Projects.RemoveRange(selectedProjects);
        db.SaveChanges();
        return CommandOutput.Success("Project deleted successfully");
    }
}