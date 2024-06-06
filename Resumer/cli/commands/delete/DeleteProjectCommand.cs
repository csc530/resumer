using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.delete;

public class DeleteProjectCommand: Command<DeleteCommandSettings>
{
    public override int Execute(CommandContext context, DeleteCommandSettings settings)
    {
        var db = new ResumeContext();
        var projects = db.Projects;
        if (!projects.Any())
            return CommandOutput.Success("No projects found");

        var selectedProjects = AnsiConsole.Prompt(new MultiSelectionPrompt<Project>()
            .Title("Select projects to delete")
            .PageSize(10)
            .AddChoices(projects));

        var prompt = selectedProjects.Count == 1
            ? $"Are you sure you want to delete this project - {selectedProjects[0].Title}?"
            : $"Are you sure you want to delete these {selectedProjects.Count} projects?";
        if (!settings.NoConfirm && !AnsiConsole.Confirm(prompt,false))
            return CommandOutput.Error(ExitCode.Canceled);
        projects.RemoveRange(selectedProjects);
        var deleted = db.SaveChanges();
        return CommandOutput.Success(deleted == 1 ? "project deleted" : $"{deleted} projects deleted");
    }
}