using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetProjectCommand: Command<GetProjectSettings>
{
    public override int Execute(CommandContext context, GetProjectSettings settings)
    {
        var db = new ResumeContext();
        var projects = db.Projects;
        if(!projects.Any())
            return CommandOutput.Success("No projects found");

        var projectsTable = Project.CreateTable(projects);
        return CommandOutput.Success(projectsTable);
    }
}

public class GetProjectSettings: OutputCommandSettings
{
    public string ProjectName { get; set; }
}