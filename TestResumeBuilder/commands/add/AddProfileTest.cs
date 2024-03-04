﻿using resume_builder.cli.commands.add;
using resume_builder.models;
using TestResumeBuilder.data;

namespace TestResumeBuilder.commands.add;

public class AddProfileTest: TestBase
{
    private readonly string[] _cmdArgs = ["add", "profile"];

    [Fact]
    public void ReturnsSuccess_WithNoArgs()
    {
        //when
        var result = TestApp.Run(_cmdArgs);
        //then
        Assert.Equal(0, result.ExitCode);
        Assert.NotEmpty(TestDb.Profiles);
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.AllOptions), MemberType = typeof(AddProfileTestData))]
    public void ReturnsSuccess_WithAllOptions(Profile profile)
    {
        //given
        var firstName = profile.FirstName;
        var middleName = profile.MiddleName;
        var lastName = profile.LastName;
        var email = profile.EmailAddress;
        var phone = profile.PhoneNumber;
        var website = profile.Website;
        var summary = profile.Summary;
        var args = CreateCmdOptions(profile);
        //when
        var result = TestApp.Run(args);
        var resultSettings = result.Settings as AddProfileSettings;
        //then
        Assert.Equal(0, result.ExitCode);
        Assert.NotNull(resultSettings);
        Assert.Multiple(() => {
            Assert.Equal(firstName, resultSettings.FirstName);
            Assert.Equal(middleName, resultSettings.MiddleName);
            Assert.Equal(lastName, resultSettings.LastName);
            Assert.Equal(email, resultSettings.EmailAddress);
            Assert.Equal(phone, resultSettings.PhoneNumber);
            Assert.Equal(website, resultSettings.Website);
            Assert.Equal(summary, resultSettings.Summary);
        });
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.AllOptions), MemberType = typeof(AddProfileTestData))]
    public void UpdatesDb_WithValid_AllOptions(Profile profile)
    {
        //given
        Assert.Empty(TestDb.Profiles);
        var firstName = profile.FirstName;
        var middleName = profile.MiddleName;
        var lastName = profile.LastName;
        var email = profile.EmailAddress;
        var phone = profile.PhoneNumber;
        var website = profile.Website;
        var summary = profile.Summary;
        var args = CreateCmdOptions(profile);
        //when
        var result = TestApp.Run(args);
        //then
        Assert.Single(TestDb.Profiles);
        Assert.Equal(0, result.ExitCode);
        var dbProfile = TestDb.Profiles.First();
        Assert.NotNull(dbProfile);
        Assert.Multiple(() => {
            Assert.Equal(firstName, dbProfile.FirstName);
            Assert.Equal(middleName, dbProfile.MiddleName);
            Assert.Equal(lastName, dbProfile.LastName);
            Assert.Equal(email, dbProfile.EmailAddress);
            Assert.Equal(phone, dbProfile.PhoneNumber);
            Assert.Equal(website, dbProfile.Website);
            Assert.Equal(summary, dbProfile.Summary);
        });
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.AllOptions), MemberType = typeof(AddProfileTestData))]
    public void ReturnsError_WithAnEmptyFirstname_AndAllOptions(Profile profile)
    {
        //given
        var args = CreateCmdOptions(string.Empty, profile.LastName, profile.EmailAddress, profile.PhoneNumber,
            profile.MiddleName, profile.Website, profile.Summary);
        //when
        //then
        Assert.ThrowsAny<Exception>(() => TestApp.Run(args));
        Assert.Empty(TestDb.Profiles);
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.WhiteSpaceStringAndProfile), MemberType = typeof(AddProfileTestData))]
    public void ReturnsError_WithWhitespaceFirstname_AndAllOptions(string firstName, Profile profile)
    {
        //given
        var args = CreateCmdOptions(firstName, profile.LastName, profile.EmailAddress, profile.PhoneNumber,
            profile.MiddleName, profile.Website, profile.Summary);
        //then
        Assert.ThrowsAny<Exception>(() => TestApp.Run(args));
        Assert.Empty(TestDb.Profiles);
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.AllOptions), MemberType = typeof(AddProfileTestData))]
    public void ReturnsError_WithAnEmptyLastname_AndAllOptions(Profile profile)
    {
        //given
        var args = CreateCmdOptions(profile.FirstName, string.Empty, profile.EmailAddress, profile.PhoneNumber,
            profile.MiddleName, profile.Website, profile.Summary);
        //when
        //then
        Assert.ThrowsAny<Exception>(() => TestApp.Run(args));
        Assert.Empty(TestDb.Profiles);
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.WhiteSpaceStringAndProfile), MemberType = typeof(AddProfileTestData))]
    public void ReturnsError_WithWhitespaceLastname_AndAllOptions(string lastname, Profile profile)
    {
        //given
        var args = CreateCmdOptions(profile.FirstName, lastname, profile.EmailAddress, profile.PhoneNumber,
            profile.MiddleName, profile.Website, profile.Summary);
        //then
        Assert.ThrowsAny<Exception>(() => TestApp.Run(args));
        Assert.Empty(TestDb.Profiles);
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.AllOptions), MemberType = typeof(AddProfileTestData))]
    public void ReturnsError_WithAnEmptyEmail_AndAllOptions(Profile profile)
    {
        //given
        var args = CreateCmdOptions(profile.FirstName, profile.LastName, string.Empty, profile.PhoneNumber,
            profile.MiddleName, profile.Website, profile.Summary);
        //when
        //then
        Assert.ThrowsAny<Exception>(() => TestApp.Run(args));
        Assert.Empty(TestDb.Profiles);
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.WhiteSpaceStringAndProfile), MemberType = typeof(AddProfileTestData))]
    public void ReturnsError_WithWhitespaceEmail_AndAllOptions(string email, Profile profile)
    {
        //given
        var args = CreateCmdOptions(profile.FirstName, profile.LastName, email, profile.PhoneNumber,
            profile.MiddleName, profile.Website, profile.Summary);
        //then
        Assert.ThrowsAny<Exception>(() => TestApp.Run(args));
        Assert.Empty(TestDb.Profiles);
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.InvalidEmails), MemberType = typeof(AddProfileTestData))]
    public void ReturnsError_WithInvalidEmail(string email, Profile profile)
    {
        //given
        var args = CreateCmdOptions(profile.FirstName, profile.LastName, null, profile.PhoneNumber,
            profile.MiddleName, profile.Website, profile.Summary);
        //then
        Assert.ThrowsAny<Exception>(() => TestApp.Run(args, "-e", email));
        Assert.Empty(TestDb.Profiles);
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.AllOptions), MemberType = typeof(AddProfileTestData))]
    public void ReturnsError_WithAnEmptyPhoneNumber_AndAllOptions(Profile profile)
    {
        //given
        var args = CreateCmdOptions(profile.FirstName, profile.LastName, profile.EmailAddress, string.Empty,
            profile.MiddleName, profile.Website, profile.Summary);
        //when
        //then
        Assert.ThrowsAny<Exception>(() => TestApp.Run(args));
        Assert.Empty(TestDb.Profiles);
    }

    [Theory]
    [MemberData(nameof(AddProfileTestData.WhiteSpaceStringAndProfile), MemberType = typeof(AddProfileTestData))]
    public void ReturnsError_WithWhitespacePhoneNumber_AndAllOptions(string phone, Profile profile)
    {
        //given
        var args = CreateCmdOptions(profile.FirstName, profile.LastName, profile.EmailAddress, phone,
            profile.MiddleName, profile.Website, profile.Summary);
        //then
        Assert.ThrowsAny<Exception>(() => TestApp.Run(args));
        Assert.Empty(TestDb.Profiles);
    }


    private string[] CreateCmdOptions(Profile profile)
    {
        var firstName = profile.FirstName;
        var middleName = profile.MiddleName;
        var lastName = profile.LastName;
        var email = profile.EmailAddress;
        var phone = profile.PhoneNumber;
        var website = profile.Website;
        var summary = profile.Summary;

        string[] args = [.._cmdArgs, "-f", firstName, "-l", lastName, "-e", email, "-p", phone];
        if(middleName != null)
            args = args.Concat(new[] { "-m", middleName }).ToArray();
        if(website != null)
            args = args.Concat(new[] { "-w", website }).ToArray();
        if(summary != null)
            args = [.. args, "-s", summary];
        return args;
    }

    private string[] CreateCmdOptions(string? firstName = null, string? lastName = null, string? email = null,
        string? phone = null,
        string? middleName = null, string? website = null, string? summary = null)
    {
        var args = _cmdArgs;
        if(firstName != null)
            args = [..args, "-f", firstName];
        if(lastName != null)
            args = [..args, "-l", lastName];
        if(email != null)
            args = [..args, "-e", email];
        if(phone != null)
            args = [..args, "-p", phone];
        if(middleName != null)
            args = [..args, "-m", middleName];
        if(website != null)
            args = [..args, "-w", website];
        if(summary != null)
            args = [.. args, "-s", summary];
        return args;
    }
}

internal class AddProfileTestData: ProfileTestData
{
    public static TheoryData<string, Profile> InvalidEmails()
    {
        var data = new TheoryData<string, Profile>();
        for(var i = 0; i < TestRepetition; i++)
            data.Add(Faker.Random.String().Replace("@", ""), GetFakeProfile());
        return data;
    }

    public static TheoryData<string, Profile> WhiteSpaceStringAndProfile()
    {
        var data = new TheoryData<string, Profile>();
        foreach(var whitespace in RandomWhiteSpaceString())
            data.Add(whitespace, GetFakeProfile());
        return data;
    }

    public static TheoryData<Profile> AllOptions()
    {
        var data = new TheoryData<Profile>();
        for(var i = TestRepetition - 1; i >= 0; i--)
            data.Add(ProfileTestData.GetFakeProfile());
        return data;
    }
}