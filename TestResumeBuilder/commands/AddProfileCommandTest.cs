using System.Collections;
using NUnit.Framework.Internal;
using resume_builder;
using resume_builder.cli.commands.add;
using resume_builder.models;
using resume_builder.models.database;
using Spectre.Console.Testing;

namespace TestResumeBuilder.commands;

[TestFixture]
public class AddProfileCommandTest : AddTest
{
	protected readonly string[] Args = { "add", "profile" };

	[Test]
	public void AddProfile_WithNoArguments_ShouldFail() => Assert.Catch(() => TestApp.Run(Args));

	[Test]
	public void AddProfile_WithMinimumArgs_ShouldPass()
	{
		var result = Run(Args, "--first", "John", "--last", "Doe", "--email", "K9qzF@example.com", "--phone",
			"555-555-5555");
		Assert.Multiple(() =>
		{
			Assert.That(result.ExitCode, Is.EqualTo(ExitCode.Success.ToInt()));
			Assert.That(new Database().GetProfiles().Count, Is.EqualTo(1));
		});
		Assert.That(result.Settings, Is.InstanceOf<AddProfileSettings>());
		Assert.That(result.Settings, Is.Not.Null);

		var profileSettings = (AddProfileSettings)(result.Settings!);
		Assert.Multiple(() =>
		{
			Assert.That(profileSettings.FirstName, Is.EqualTo("John"));
			Assert.That(profileSettings.LastName, Is.EqualTo("Doe"));
			Assert.That(profileSettings.EmailAddress, Is.EqualTo("K9qzF@example.com"));
			Assert.That(profileSettings.PhoneNumber, Is.EqualTo("555-555-5555"));
		});
	}

	[Test]
	public void AddProfile_WithoutFirstName_ShouldFail() => Assert.Catch(() =>
		Run(Args, "--last", "Doe", "--email", "K9qzF@example.com", "--phone", "555-555-5555"));

	[Test]
	public void AddProfile_WithoutLastName_ShouldFail() => Assert.Catch(() =>
		Run(Args, "--first", "John", "--email", "K9qzF@example.com", "--phone", "555-555-5555"));

	[Test]
	public void AddProfile_WithoutEmailAddress_ShouldFail() => Assert.Catch(() =>
		Run(Args, "--first", "John", "--last", "Doe", "--phone", "555-555-5555"));

	[Test]
	public void AddProfile_WithoutPhoneNumber_ShouldFail() => Assert.Catch(() =>
		Run(Args, "--first", "John", "--last", "Doe", "--email", "K9qzF@example.com"));

	[Test]
	[TestCase("")]
	[TestCase(null)]
	public void AddProfile_WithInvalidEmailAddress_ShouldFail(string? email) => Assert.Catch(() =>
		Run(Args, "--first", "John", "--last", "Doe", "--email", email, "--phone", "555-555-5555"));

	[Test]
	[TestCase("")]
	[TestCase(null)]
	public void AddProfile_WithInvalidPhoneNumber_ShouldFail(string? phone) => Assert.Catch(() =>
		Run(Args, "--first", "John", "--last", "Doe", "--email", "K9qzF@example.com", "--phone", phone));

	[Test]
	[TestCase("")]
	[TestCase(null)]
	public void AddProfile_WithInvalidFirstName_ShouldFail(string? name) => Assert.Catch(() =>
		Run(Args, "--first", name, "--last", "Doe", "--email", "K9qzF@example.com", "--phone", "555-555-5555"));

	[Test]
	[TestCase("")]
	[TestCase(null)]
	public void AddProfile_WithInvalidLastName_ShouldFail(string? name) => Assert.Catch(() =>
		Run(Args, "--first", "John", "--last", name, "--email", "K9qzF@example.com", "--phone", "555-555-5555"));
}