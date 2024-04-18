using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Resumer.models;

public class Resume
{
    private string _name;
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name
    {
        get => _name;
        [MemberNotNull(nameof(_name))] set => _name = value.Trim();
    }

    public DateTime DateCreated { get; init; } = DateTime.Now;

    public required Profile Profile { get; set; }
    public List<Job> Jobs { get; set; } = [];
    public List<Skill> Skills { get; set; } = [];
    public List<Project> Projects { get; set; } = [];

    public string ExportToTxt()
    {
        const string sectionBreak = "==================================================";
        var sb = new StringBuilder();
        sb.AppendLine(Profile.FullName);
        if(Profile.Location != null)
            sb.AppendLine(Profile.Location);
        sb.AppendLine(Profile.EmailAddress);
        sb.AppendLine(Profile.PhoneNumber);
        sb.AppendLine();

        if(Profile.Objective != null)
            sb.AppendLine("PROFESSIONAL SUMMARY")
                .AppendLine(sectionBreak)
                .AppendLine(Profile.Objective);

        sb.AppendLine("WORK EXPERIENCE")
            .AppendLine(sectionBreak);
        foreach (var job in from j in Jobs orderby j.StartDate descending select j)
            sb.AppendLine(job.Title)
                .AppendLine(job.Company /*+ " - " + job.Location*/)
                .AppendLine(Utility.PrintDuration(job.StartDate, job.EndDate))
                .AppendJoin("\n + ", job.Description);

        sb.AppendLine("SKILLS")
            .AppendLine(sectionBreak);
        foreach (var skill in Skills.OrderBy(skill=>skill.Name))
            sb.AppendLine(skill.Name);

        sb.AppendLine("PROJECTS")
            .AppendLine(sectionBreak);
        foreach (var project in Projects)
            sb.AppendLine(project.Title)
                .AppendLine(Utility.PrintDuration(project.StartDate, project.EndDate))
                .AppendLine(project.Description);

        sb.AppendLine("REFERENCES")
            .AppendLine(sectionBreak)
            .AppendLine("Available upon request");

        return sb.ToString();
    }
}