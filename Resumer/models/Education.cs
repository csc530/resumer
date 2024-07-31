using System.ComponentModel;

namespace Resumer.models;

public class Education
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string School { get; set; }
    public Certification Degree { get; set; }
    [Description("The level or field of study")]
    public string FieldOfStudy { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public double? GradePointAverage { get; set; }
    public string? Location { get; set; }
    public string? AdditionalInformation { get; set; }
    public List<string> Courses { get; set; } = [];

    public override string ToString() => $"{School} ({Degree.Print()}) {FieldOfStudy} - {StartDate:yyyy-M-d} - {(EndDate == null ? "present" : EndDate.Value.ToString("yyyy-M-d"))}";
}

public enum Certification
{
    None,
    Certificate,
    SecondarySchoolDiploma,
    ALevels,
    Degree,
    Diploma,
    HonoursBachelor,
    Bachelors,
    Associate,
    Master,
    Doctorate
}