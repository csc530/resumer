using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.Sqlite;
using resume_builder.models;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = resume_builder.models.Profile;

namespace resume_builder.cli.commands.add;

public sealed class AddProfileCommand : Command<AddProfileSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] AddProfileSettings settings)
    {
        var firstName = settings.FirstName;
        var lastName = settings.LastName;
        var middleName = settings.MiddleName;
        var phoneNumber = settings.PhoneNumber;
        var emailAddress = settings.EmailAddress;
        var website = settings.Website;
        var summary = settings.Summary;

        if(settings.PromptUser)
        {
            var firstNamePrompt = new TextPrompt<string>("First name: ");
        }

        var profile = new Profile(settings.FirstName, settings.LastName, settings.PhoneNumber, settings.EmailAddress);
        profile.Summary = settings.Summary;
        profile.Website = settings.Website;
        ResumeContext database = new();
        try
        {
            database.Profiles.Add(profile);
            database.SaveChanges();
        }
        catch(Exception e)
        {
            return Globals.PrintError(settings, e);
        }

        AnsiConsole.MarkupLine($"✅ profile: [BOLD]{profile.FullName}[/] added");
        return ExitCode.Success.ToInt();
    }
}

public class AddProfileSettings : AddCommandSettings
{
    public bool PromptUser =>
        (FirstName.IsBlank() && LastName.IsBlank() && MiddleName.IsBlank() && PhoneNumber.IsBlank() &&
         EmailAddress.IsBlank() && Website.IsBlank() && Summary.IsBlank())
        ||
        (!FirstName.IsBlank() && !LastName.IsBlank() && !MiddleName.IsBlank() && !PhoneNumber.IsBlank() &&
         !EmailAddress.IsBlank() && !Website.IsBlank() && !Summary.IsBlank())
        ||
        Interactive;

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