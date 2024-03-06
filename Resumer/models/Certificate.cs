namespace Resumer.models;

public class Certificate
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string? Issuer { get; set; }
    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public string? Description { get; set; }
    public Uri? Url { get; set; }
    public string? CredentialId { get; set; }

    public Certificate()
    {
        Id = Guid.NewGuid();
    }
}