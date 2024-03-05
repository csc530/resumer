using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using PhoneNumbers;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.add;

public sealed class AddProfileCommand: Command<AddProfileSettings>
{
    public override int Execute(CommandContext context, AddProfileSettings settings)
    {
        var firstName = settings.FirstName;
        var lastName = settings.LastName;
        var middleName = settings.MiddleName;
        var phoneNumber = settings.PhoneNumber;
        var emailAddress = settings.EmailAddress;
        var website = settings.Website;
        var summary = settings.Summary;

        var phoneNumberUtil = PhoneNumberUtil.GetInstance();

        if(settings.PromptUser)
        {
            var firstNamePrompt = RenderableFactory.CreateTextPrompt("First name: ", firstName);
            var lastNamePrompt = RenderableFactory.CreateTextPrompt("Last name: ", lastName);
            var middleNamePrompt = RenderableFactory.CreateTextPrompt("Middle name: ", middleName).AllowEmpty();
            var phoneNumberPrompt = RenderableFactory.CreateTextPrompt("Phone number: ", phoneNumber)
                                                     .Validate(phone => PhoneNumberUtil.IsViablePhoneNumber(phone)
                                                          ? ValidationResult.Success()
                                                          : ValidationResult.Error("Invalid phone number"));
            var emailAddressPrompt = RenderableFactory.CreateTextPrompt("Email address: ", emailAddress).Validate(
                //from https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
                email => string.IsNullOrWhiteSpace(email) || Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Invalid email address"));
            var websitePrompt = RenderableFactory.CreateTextPrompt("Website: ", website).AllowEmpty();
            var summaryPrompt = RenderableFactory.CreateTextPrompt("Summary: ", summary).AllowEmpty();

            firstName = AnsiConsole.Prompt(firstNamePrompt);
            lastName = AnsiConsole.Prompt(lastNamePrompt);
            middleName = AnsiConsole.Prompt(middleNamePrompt);
            var phone = phoneNumberUtil.Parse(AnsiConsole.Prompt(phoneNumberPrompt),
                CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            phoneNumber = $"+{phone.CountryCode} {phone.NationalNumber}{(phone.HasExtension ? $",{phone.Extension}" : "")}";
            emailAddress = AnsiConsole.Prompt(emailAddressPrompt);
            website = AnsiConsole.Prompt(websitePrompt);
            summary = AnsiConsole.Prompt(summaryPrompt);
        }

        var profile = new Profile(firstName, lastName, phoneNumber, emailAddress)
        {
            MiddleName = middleName,
            Website = website,
            Summary = summary
        };
        profile.Summary = settings.Summary;
        profile.Website = settings.Website;
        ResumeContext database = new();

        database.Profiles.Add(profile);
        database.SaveChanges();

        AnsiConsole.MarkupLine($"✅ profile: [BOLD]{profile.FullName}[/] added");
        return ExitCode.Success.ToInt();
    }
}

public class AddProfileSettings: AddCommandSettings
{
    public bool PromptUser =>
        (FirstName.IsBlank() && LastName.IsBlank() && MiddleName.IsBlank() && PhoneNumber.IsBlank() &&
         EmailAddress.IsBlank() && Website.IsBlank() && Summary.IsBlank())
        ||
        Interactive;

    [CommandOption("-f|--first <FirstName>")]
    public string? FirstName { get; set; }

    [CommandOption("-m|--middle <MiddleInitials>")]
    public string? MiddleName { get; init; }

    [CommandOption("-l|--last <LastName>")]
    public string? LastName { get; set; }

    [CommandOption("-p|--phone <PhoneNumber>")]
    public string PhoneNumber { get; set; }

    [CommandOption("-e|--email <EmailAddress>")]
    public string? EmailAddress { get; set; }

    [CommandOption("-w|--website <Website>")]
    public string? Website { get; set; }

    [CommandOption("-s|--summary <Summary>")]
    public string? Summary { get; set; }

    public override ValidationResult Validate()
    {
        if(PromptUser)
            return ValidationResult.Success();
        if(string.IsNullOrWhiteSpace(FirstName))
            return ValidationResult.Error(FirstName == null ? "First name is required" : "First name cannot be empty");
        if(string.IsNullOrWhiteSpace(LastName))
            return ValidationResult.Error(LastName == null ? "Last name is required" : "Last name cannot be empty");

        if(string.IsNullOrWhiteSpace(EmailAddress))
            return ValidationResult.Error("Email address is required");
        if(!EmailAddress.Contains('@'))
            return ValidationResult.Error("Email address invalid: must contain '@'");

        if(string.IsNullOrWhiteSpace(PhoneNumber))
            return ValidationResult.Error("Phone number is required");

        return ValidationResult.Success();
    }
}