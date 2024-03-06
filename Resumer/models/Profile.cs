using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Resumer.models;

[PrimaryKey(nameof(FirstName), nameof(LastName), nameof(EmailAddress), nameof(PhoneNumber))]
public class Profile
{
    private string _emailAddress;
    private string _firstName;
    private string _lastName;
    private string _phoneNumber;

    public Profile(string firstName, string lastName, string phoneNumber, string emailAddress)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if(value.IsBlank())
                throw new ArgumentException("First name cannot be blank");
            _firstName = value;
        }
    }

    public string? MiddleName { get; set; }

    [Key]
    public string LastName
    {
        get => _lastName;
        set
        {
            if(value.IsBlank())
                throw new ArgumentException("Last name cannot be blank");
            _lastName = value;
        }
    }

    public string PhoneNumber

    {
        get => _phoneNumber;
        set
        {
            if(value.IsBlank())
                throw new ArgumentException("Phone number cannot be blank");
            _phoneNumber = value;
        }
    }

    public string EmailAddress

    {
        get => _emailAddress;
        set
        {
            if(value.IsBlank())
                throw new ArgumentException("Email address cannot be blank");
            _emailAddress = value;
        }
    }

    public string? Location { get; set; }

    public List<Education> Education { get; set; }
    public List<string> Interests { get; set; }
    public List<string> Languages { get; set; }
    public List<Certificate> Certifications { get; set; }

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
    public string WholeName => $"{FirstName} {MiddleName} {LastName}";

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
}