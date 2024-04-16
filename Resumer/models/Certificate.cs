namespace Resumer.models;

public class Certificate
{
    private string _name;
    private string? _issuer;
    private string? _description;
    private string? _credentialId;

    public Guid Id { get; init; }

    public string Name
    {
        get => _name;
        set => _name = value.Trim();
    }

    public string? Issuer
    {
        get => _issuer;
        set => _issuer = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public string? Description
    {
        get => _description;
        set => _description = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public Uri? Url { get; set; }

    public string? CredentialId
    {
        get => _credentialId;
        set => _credentialId = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public Certificate()
    {
        Id = Guid.NewGuid();
    }
}