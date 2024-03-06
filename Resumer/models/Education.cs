using Microsoft.EntityFrameworkCore;

namespace Resumer.models;

[PrimaryKey(nameof(School), nameof(Degree), nameof(StartDate))]
public class Education
{
    public string School { get; set; }
    public string Degree { get; set; }
    public string? FieldOfStudy { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public float? GradePointAverage { get; set; }
    public string? Location { get; set; }
    public string? AdditionalInformation { get; set; }
}