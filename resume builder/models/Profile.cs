using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace resume_builder.models;

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

    [Phone()]
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

    [EmailAddress]
    public string EmailAddress

    {
        get => _emailAddress;
        set
        {
            if(value.IsBlank())
                throw new ArgumentException("Email address cannot be blank");
            if(!value.Contains('@'))
                throw new ArgumentException($"Email address invalid: it must contain '@': {value}");
            _emailAddress = value;
        }
    }

    public string? Website { get; set; }
    public string? Summary { get; set; }
}