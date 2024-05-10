using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Resumer.models;

public class Profile
{
    private string _emailAddress;
    private string _firstName;
    private string _lastName;
    private string _phoneNumber;


    public Guid Id { get; init; } = Guid.NewGuid();

    public required string FirstName
    {
        get => _firstName;
        [MemberNotNull(nameof(_firstName))]
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be blank");
            _firstName = value.Trim();
        }
    }

    public string? MiddleName { get; set; }

    public required string LastName
    {
        get => _lastName;
        [MemberNotNull(nameof(_lastName))]
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Last name cannot be blank");
            _lastName = value.Trim();
        }
    }

    public required string PhoneNumber
    {
        get => _phoneNumber;
        [MemberNotNull(nameof(_phoneNumber))]
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number cannot be blank");
            _phoneNumber = value.Trim();
        }
    }

    public required string EmailAddress
    {
        get => _emailAddress;
        [MemberNotNull(nameof(_emailAddress))]
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email address cannot be blank");
            _emailAddress = value.Trim();
        }
    }

    public string? Location { get; set; }

    public List<Education> Education { get; set; } = [];
    public List<string> Interests { get; set; } = [];
    public List<string> Languages { get; set; } = [];
    public List<Certificate> Certifications { get; set; } = [];

    public string? Website { get; set; }
    public string? Objective { get; set; }


    /// <summary>
    /// Gets the name whole name - first, middle and last names
    /// </summary>
    /// <value>
    /// Their whole name
    /// </value>
    /// 
    [NotMapped]
    public string WholeName => $"{FirstName} {(MiddleName == null ? "" : $"{MiddleName} ")}{LastName}";

    /// <summary>
    /// Gets the full name - first and last names
    /// </summary>
    /// <value>
    /// Their full name.
    /// </value>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Gets the initials - first, middle and last name initials (initial letter/character)
    /// </summary>
    /// <value>
    /// Their full initials.
    /// </value>
    [NotMapped]
    public string Initials => $"{FirstName[0]}{MiddleName?[0]}{LastName[0]}";

    public override string ToString() => $"{WholeName} - {EmailAddress} - {PhoneNumber}";
}