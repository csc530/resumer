using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Command = Spectre.Console.Cli.Command;

namespace Resumer.cli.commands.add;

internal sealed class AddProjectCommand: Command
{
    public override int Execute(CommandContext context)
    {
        var projectName = AnsiConsole.Ask<string>("Project Name:");
        var projectType = AnsiConsole.Prompt(Utility.SimplePrompt<string>("Project Type:"));
        var projectDescription = AnsiConsole.Prompt(Utility.SimplePrompt<string>("Project Description:"));
        var projectDetails = new List<string>();
        projectDetails.AddFromPrompt("Project Details (point form):");

        Uri? projectUri = null;
        string? projectUrl;
        bool isValidUri = false;
        do
        {
            projectUrl = AnsiConsole.Prompt(Utility.SimplePrompt<string?>("Project URL:"));
            isValidUri = Uri.TryCreate(projectUrl, UriKind.Absolute, out projectUri);
            if(!isValidUri && projectUrl != null)
                AnsiConsole.MarkupLine("[red]Invalid URL. Please enter a valid URL.[/]");
        } while(projectUrl != null && !isValidUri);

        var projectStartDate = AnsiConsole.Prompt(Utility.SimplePrompt<DateOnly?>("Start Date:"));
        var projectEndDate = AnsiConsole.Prompt(Utility.SimplePrompt<DateOnly?>("End Date:"));

        var project = new Project(projectName)
        {
            Type = projectType,
            Description = projectDescription,
            Details = projectDetails,
            Link = projectUri,
            StartDate = projectStartDate,
            EndDate = projectEndDate,
        };

        var db = new ResumeContext();
        db.Projects.Add(project);
        db.SaveChanges();
        return CommandOutput.Success($"✅ Added project {projectName}");
    }
}