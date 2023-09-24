using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = resume_builder.models.Profile;

namespace resume_builder.cli.commands.add;

public sealed class AddProfileCommand : Command<AddProfileSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] AddProfileSettings settings)
	{
		var profile = new Profile(settings.FirstName, settings.LastName, settings.PhoneNumber, settings.EmailAddress);
		profile.Summary = settings.Summary;
		profile.Website = settings.Website;
		Database database = new();
		database.AddProfile(profile);
		AnsiConsole.MarkupLine($"✅ profile: [BOLD]{profile.FullName}[/] added");
		return ExitCode.Success.ToInt();
	}
}

public class AddProfileSettings : CommandSettings
{
	[CommandOption("-f|--first <FirstName>")]
	public string FirstName { get; set; }

	[CommandOption("-m|--middle <MiddleInitials>")]
	public string? MiddleName { get; init; }

	[CommandOption("-l|--last <LastName>")]
	public string LastName { get; set; }

	[CommandOption("-p|--phone <PhoneNumber>")]
	public string PhoneNumber { get; set; }

	[CommandOption("-e|--email <EmailAddress>")]
	public string EmailAddress { get; set; }

	[CommandOption("-w|--website <Website>")]
	public string? Website { get; set; }

	[CommandOption("-s|--summary <Summary>")]
	public string? Summary { get; set; }

	public override ValidationResult Validate()
	{
		if(string.IsNullOrWhiteSpace(FirstName))
			return ValidationResult.Error(FirstName == null ? "First name is required" : "First name cannot be empty");
		if(string.IsNullOrWhiteSpace(EmailAddress))
			return ValidationResult.Error("Email address is required");
		if(!EmailAddress.Contains('@'))
			return ValidationResult.Error("Email address invalid: must contain '@'");
		if(string.IsNullOrWhiteSpace(PhoneNumber))
			return ValidationResult.Error("Phone number is required");
		return ValidationResult.Success();
	}
}