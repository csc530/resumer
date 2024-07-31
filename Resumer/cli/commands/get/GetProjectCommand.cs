using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetProjectCommand: Command<GetProjectSettings>
{
    public override int Execute(CommandContext context, GetProjectSettings settings)
    {
        var db = new ResumeContext();
        var projects = db.Projects.ToList();
        if(projects.Count == 0)
            return CommandOutput.Success("No projects found");

        var table = settings.CreateTable();
        if(table == null)
            projects.ForEach(project => AnsiConsole.WriteLine(project.ToString()));
        else
        {
            projects.ForEach(project => GetProjectSettings.AddProjectToTable(table, project));
            AnsiConsole.Write(table);
        }

        return CommandOutput.Success();
    }
}

public class GetProjectSettings: OutputCommandSettings
{
    public string ProjectName { get; set; }

    public Table? CreateTable()
    {
        var table = CreateTable("Projects");
        if(table == null) return table;

        table.AddColumn("Title");
        table.AddColumn("Type");
        table.AddColumn("Description");
        table.AddColumn("Details");
        table.AddColumn("Link");
        table.AddColumn("Start Date");
        table.AddColumn("End Date");

        return table;
    }

    /// <summary>
    /// adds a <see cref="Project"/> to <see cref="Table"/>
    /// </summary>
    /// <param name="table">table created by <see cref="CreateTable"/></param>
    /// <returns>table with the newly added project</returns>
    public static void AddProjectToTable(Table table, Project project)
    {
        table.AddRow(
            project.Title,
            project.Type.Print(),
            project.Description.Print(),
            project.Details.Print(),
            project.Link.Print(),
            project.StartDate.Print(),
            project.EndDate.Print()
        );
    }
}