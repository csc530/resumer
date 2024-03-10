using System.Globalization;
using System.Text.RegularExpressions;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.add;

public sealed class AddProfileCommand: Command<AddProfileSettings>
{
    public override int Execute(CommandContext context, AddProfileSettings settings)
    {
        var firstNamePrompt = RenderableFactory.CreateTextPrompt<string>("First name: ");
        var lastNamePrompt = RenderableFactory.CreateTextPrompt<string>("Last name: ");
        var middleNamePrompt = RenderableFactory.CreateTextPrompt<string?>("Middle name: ").AllowEmpty();
        var phoneNumberPrompt = RenderableFactory.CreateTextPrompt<string>("Phone number: ");
        var emailAddressPrompt = RenderableFactory.CreateTextPrompt<string>("Email address: ").Validate(
            //from https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
            email => string.IsNullOrWhiteSpace(email) || Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))
                ? ValidationResult.Success()
                : ValidationResult.Error("Invalid email address"));
        var websitePrompt = RenderableFactory.CreateTextPrompt<string?>("Website: ").AllowEmpty();
        var summaryPrompt = RenderableFactory.CreateTextPrompt<string?>("Summary: ").AllowEmpty();

        var firstName = AnsiConsole.Prompt(firstNamePrompt);
        var lastName = AnsiConsole.Prompt(lastNamePrompt);
        var middleName = AnsiConsole.Prompt(middleNamePrompt);
        var phoneNumber = (AnsiConsole.Prompt(phoneNumberPrompt));
        var emailAddress = AnsiConsole.Prompt(emailAddressPrompt);
        var website = AnsiConsole.Prompt(websitePrompt);
        var summary = AnsiConsole.Prompt(summaryPrompt);


        var profile = new Profile(firstName, lastName, phoneNumber, emailAddress)
        {
            MiddleName = middleName,
            Website = website,
            Objective = summary
        };

        ResumeContext database = new();

        database.Profiles.Add(profile);
        database.SaveChanges();

        return CommandOutput.Success($"✅ profile: [BOLD]{profile.FullName}[/] added");
    }
}

public class AddProfileSettings: AddCommandSettings
{
}