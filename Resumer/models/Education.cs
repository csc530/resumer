using Microsoft.EntityFrameworkCore;

namespace Resumer.models;

public class Education
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string School { get; set; }
    public string Degree { get; set; }
    public string? FieldOfStudy { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public float? GradePointAverage { get; set; }
    public string? Location { get; set; }
    public string? AdditionalInformation { get; set; }
}