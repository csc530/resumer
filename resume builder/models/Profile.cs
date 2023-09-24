using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Sqlite;

namespace resume_builder.models;

[Table("profile")]
public class Profile
{
	[Column("firstName")] public string FirstName { get; set; }
	[Column("middleName")] public string? MiddleName { get; set; }
	[Column("lastName")] public string LastName { get; set; }

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

	[Phone(), Column("phoneNumber")] public string PhoneNumber { get; protected set; }

	[EmailAddress] [Column("email")] public string EmailAddress { get; protected set; }

	[Column("website")] public string? Website { get; set; }
	[Column("summary")] public string? Summary { get; set; }

	public Profile(string firstName, string lastName, string phoneNumber, string emailAddress)
	{
		SetFirstName(firstName);
		SetLastName(lastName);
		SetPhoneNumber(phoneNumber);
		SetEmailAddress(emailAddress);
	}

	private void SetEmailAddress(string emailAddress)
	{
		if(!string.IsNullOrWhiteSpace(emailAddress) && emailAddress.Contains('@'))
			EmailAddress = emailAddress;
		else
			throw new ArgumentException("Email address invalid: must contain '@'", nameof(emailAddress));
	}

	private void SetPhoneNumber(string phoneNumber)
	{
		if(!string.IsNullOrWhiteSpace(phoneNumber))
			PhoneNumber = phoneNumber;
		else
			throw new ArgumentException("Phone number cannot be null or empty", nameof(phoneNumber));
	}

	private void SetLastName(string lastName)
	{
		if(string.IsNullOrWhiteSpace(lastName))
			throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));
		LastName = lastName;
	}

	private void SetFirstName(string? firstName)
	{
		if(!string.IsNullOrWhiteSpace(firstName))
			FirstName = firstName!;
		else
			throw new ArgumentException("First name cannot be null or empty", nameof(firstName));
	}

	public static Profile? ParseProfilesFromQuery(SqliteDataReader sqliteDataReader)
	{
		if(!sqliteDataReader.HasRows)
			return null;
		var firstName = sqliteDataReader.GetNullableValue<string>("firstName");
		var lastName = sqliteDataReader.GetNullableValue<string>("lastName");
		var phoneNumber = sqliteDataReader.GetNullableValue<string>("phoneNumber");
		var emailAddress = sqliteDataReader.GetNullableValue<string>("email");
		var website = sqliteDataReader.GetNullableValue<string>("website");
		var summary = sqliteDataReader.GetNullableValue<string>("summary");
		return new Profile(firstName, lastName, phoneNumber, emailAddress)
		{
			Website = website,
			Summary = summary
		};
	}
}