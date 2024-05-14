using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

internal sealed class AddProjectCommand: Command
{
    public override int Execute(CommandContext context)
    {
        var projectName = AnsiConsole.Ask<string>("Project Name:");
        var projectType = AnsiConsole.Prompt(new SimplePrompt<string>("Project Type:"));
        var projectDescription = AnsiConsole.Prompt(new SimplePrompt<string>("Project Description:"));
        var projectDetails = new List<string>();
        projectDetails.AddFromPrompt("Project Details (point form):");

        Uri? projectUri = null;
        string? projectUrl;
        bool isValidUri = false;
        do
        {
            projectUrl = AnsiConsole.Prompt(new SimplePrompt<string?>("Project URL:"));
            isValidUri = Uri.TryCreate(projectUrl, UriKind.Absolute, out projectUri);
            if(!isValidUri && projectUrl != null)
                AnsiConsole.MarkupLine("[red]Invalid URL. Please enter a valid URL.[/]");
        } while(projectUrl != null && !isValidUri);

        var projectStartDate = AnsiConsole.Prompt(new SimplePrompt<DateOnly?>("Start Date:"));
        var projectEndDate = AnsiConsole.Prompt(new SimplePrompt<DateOnly?>("End Date:"));

        var project = new Project
        {
            Title = projectName,
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