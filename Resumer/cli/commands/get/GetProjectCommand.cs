using Microsoft.EntityFrameworkCore;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
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

        var table = settings.CreateTable<Project>("Projects")?.AddObjects(db.Projects);
        if(table == null)
            projects.ForEachAsync(project => AnsiConsole.WriteLine(project.ToString())).Wait();
        else
            AnsiConsole.Write(table);
        return CommandOutput.Success();
    }
}

public class GetProjectSettings: OutputCommandSettings
{
    public string ProjectName { get; set; }
}