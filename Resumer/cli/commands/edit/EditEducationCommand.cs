using System.Runtime.ConstrainedExecution;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Command = Spectre.Console.Cli.Command;

namespace Resumer.cli.commands.edit;

public class EditEducationCommand: Command
{
    public override int Execute(CommandContext context)
    {
        var db = new ResumeContext();
        var educationList = db.Education.ToList();

        AnsiConsole.WriteLine("select an education to edit...");
        var education = AnsiConsole.Prompt(
            new SelectionPrompt<Education>()
                .AddChoices(educationList)
                .WrapAround()
        );
        AnsiConsole.WriteLine($"Editing: {education}");
        
        var school = AnsiConsole.Prompt(new TextPrompt<string>("School").DefaultValue(education.School));
        var degree =
            AnsiConsole.Prompt(new SelectionPrompt<Certification>().AddChoices(Enum.GetValues<Certification>()));
        var location = AnsiConsole.Prompt(new TextPrompt<string?>("Location:").DefaultValue(education.Location));
        var gpa = AnsiConsole.Prompt(
            new TextPrompt<double?>("Grade point average:").DefaultValue(education.GradePointAverage));
        var field = AnsiConsole.Prompt(
            new TextPrompt<string>("Field (or level) of study:").DefaultValue(education.FieldOfStudy));
        var start = AnsiConsole.Prompt(new TextPrompt<DateOnly>("Start date:").DefaultValue(education.StartDate));
        var end = AnsiConsole.Prompt(new TextPrompt<DateOnly?>("End date:").DefaultValue(education.EndDate));
        AnsiConsole.WriteLine("\nHighlight certain courses, classes, subjects, projects, etc.");
        AnsiConsole.MarkupLine("Press [bold]Enter[/] to skip.");
        var courses = new List<string>();
        courses.EditFromPrompt("Course:");
        var additionalInformation =
            AnsiConsole.Prompt(
                new TextPrompt<string?>("Additional information:").DefaultValue(education.AdditionalInformation));


        education.School = school;
        education.Degree = degree;
        education.FieldOfStudy = field;
        education.StartDate = start;
        education.EndDate = end;
        education.GradePointAverage = gpa;
        education.Location = location;
        education.AdditionalInformation = additionalInformation;
        education.Courses = courses;

        db.SaveChanges();
        return CommandOutput.Success("education updated successfully");
    }
}