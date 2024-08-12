using System.Text.RegularExpressions;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.add;

public sealed partial class AddProfileCommand : AddCommand
{
    protected override int AddItem(CommandContext context, AddCommandSettings settings)
    {
        var emailAddressPrompt = new TextPrompt<string>("Email address:")
            .ValidationErrorMessage("Invalid email address: email should contain '@' and a domain (e.g. @example.ca).")
            //from https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
            .Validate(email => string.IsNullOrWhiteSpace(email) || !EmailRegex().IsMatch(email)
                ? ValidationResult.Error()
                : ValidationResult.Success());

        var firstName = AnsiConsole.Ask<string>("First name:");
        var lastName = AnsiConsole.Ask<string>("Last name:");
        var middleName = AnsiConsole.Prompt(Utility.SimplePrompt<string>("Middle name: "));

        var phoneNumber = AnsiConsole.Ask<string>("Phone number:");
        var emailAddress = AnsiConsole.Prompt(emailAddressPrompt);

        var location = AnsiConsole.Prompt(Utility.SimplePrompt<string>("Location:"));
        var website = AnsiConsole.Prompt(Utility.SimplePrompt<string>("Website:"));
        var objective = AnsiConsole.Prompt(Utility.SimplePrompt<string>("Objective:"));

        AnsiConsole.MarkupLine("Add your interests and languages. Press [bold]Enter[/] to skip.");

        var interests = new List<string>();
        interests.AddFromPrompt("Interests:");
        var languages = new List<string>();
        languages.AddFromPrompt("Languages:");



        var profile = new Profile
        {
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = emailAddress,
            PhoneNumber = phoneNumber,

            Objective = objective,
            MiddleName = middleName,
            Languages = languages,
            Interests = interests,
            Location = location,
            Website = website,
        };

        ResumeContext database = new();
        database.Profiles.Add(profile);
        database.SaveChanges();

        return CommandOutput.Success($"✅ profile: [BOLD]{profile.FullName}[/] added");
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, "en-CA")]
    private static partial Regex EmailRegex();
}

