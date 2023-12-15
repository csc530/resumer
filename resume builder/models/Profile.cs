using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace resume_builder.models;

[PrimaryKey(nameof(FirstName), nameof(MiddleName), nameof(LastName))]
public class Profile
{
    private string _emailAddress;
    private string _phoneNumber;
    private string _firstName;
    private string _lastName;

    public string FirstName
    {
        get => _firstName;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
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
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _lastName = value;
        }
    }

    public string WholeName
    {
        get => $"{FirstName} {MiddleName} {LastName}";
        set
        {
            var parts = value.Split(' ');
            FirstName = parts[0];
            MiddleName = parts[1];
            LastName = parts[2];
        }
    }

    public string FullName
    {
        get => $"{FirstName} {LastName}";
        set
        {
            var parts = value.Split(' ');
            FirstName = parts[0];
            LastName = parts[1];
        }
    }

    public string Initials => $"{FirstName[..1]}{LastName[..1]}";

    [Phone()]
    public string PhoneNumber

    {
        get => _phoneNumber;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _phoneNumber = value;
        }
    }

    [EmailAddress]
    public string EmailAddress

    {
        get => _emailAddress;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            if(!value.Contains('@'))
                throw new ArgumentException("Email address invalid: it must contain '@'", nameof(value));
            _emailAddress = value;
        }
    }

    public string? Website { get; set; }
    public string? Summary { get; set; }

    public Profile(string firstName, string lastName, string phoneNumber, string emailAddress)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
    }
}