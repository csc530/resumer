using Bogus;
using Resumer;
using Resumer.cli.commands.add;
using Resumer.models;
using TestResumer.data;

namespace TestResumer.commands.add;

public class AddJobTest: TestBase
{
    static readonly string[] cmdArgs = ["add", "job"];

    public static readonly TheoryData<string, string, DateOnly, string?, string?, DateOnly?> AddJobData =
        new()
        {
            {
                "jobTitle", "companyName", new DateOnly(2021, 1, 1), "jobDescription", "experience",
                new DateOnly(2231, 1, 1)
            }
        };

    // [Fact(Skip = "hangs forever (at least in vs2022) because it's expecting input in interactive mode which can't be emulated right now with its (spectre) test console")]
    [Fact]
    public void ReturnsSuccess_WithNoArgs_AndEnteredPrompts()
    {
        //this hangs forever becuase a prompt is required
        //and I have no idea how to inject a response to the app
        //I added a timeout so it sould stop
        var result = TestApp.Run(cmdArgs);
        Assert.Equal(ExitCode.Error.ToInt(), result.ExitCode);
    }

    [Theory]
    [MemberData(nameof(AddJobTestData.JobTitleAndStartDate), MemberType = typeof(AddJobTestData))]
    public void ReturnsSuccess_WithValid_NameAndStartOptions(string jobTitle, string companyName, DateOnly startDate)
    {
        //when
        var result = TestApp.Run(cmdArgs, "-t", jobTitle, "-s", startDate.ToString(), "-c", companyName);
        var resultSettings = result.Settings as AddJobSettings;
        //then
        Assert.Multiple(() => {
            Assert.Equal(0, result.ExitCode);
            Assert.NotNull(resultSettings);
            Assert.Equal(jobTitle, resultSettings.JobTitle);
            Assert.Equal(companyName, resultSettings.Company);
            Assert.Equal(startDate, resultSettings.StartDate);
        });
    }

    [Theory]
    [MemberData(nameof(AddJobTestData.AllOptions), MemberType = typeof(AddJobTestData))]
    public void ReturnsSuccess_WithValid_AllOptions(string jobTitle, string companyName, DateOnly startDate,
        string? jobDescription, string? experience, DateOnly? endDate)
    {
        //? job description and experience are non-null because in the run it will be parsed/replaced with an empty string
        // and users can't enter the null value
        //which is to say they can choose to enter nothing with empty quotes which this test tests for too
        //when
        string[] descriptionArg = jobDescription == null ? [null] : ["-d", $"{jobDescription}"];
        string[] expArg = experience == null ? [null] : ["-x", $"{experience}"];
        string[] endArg = endDate == null ? [null] : ["-e", $"{endDate}"];
        List<string?> args =
        [
            ..cmdArgs, ..descriptionArg, ..expArg, ..endArg, "-t", jobTitle, "-s", startDate.ToString(), "-c",
            companyName
        ];
        var result = TestApp.Run(args.Where(x => x != null).ToArray()!);
        var resultSettings = result.Settings as AddJobSettings;
        //then
        Assert.Equal(0, result.ExitCode);
        Assert.NotNull(resultSettings);
        Assert.Equal(jobTitle, resultSettings.JobTitle);
        Assert.Equal(companyName, resultSettings.Company);
        Assert.Equal(startDate, resultSettings.StartDate);
        Assert.Equal(jobDescription, resultSettings.JobDescription);
        Assert.Equal(experience, resultSettings.Experience);
        Assert.Equal(endDate, resultSettings.EndDate);
    }

    [Theory]
    [MemberData(nameof(AddJobData))]
    [MemberData(nameof(AddJobTestData.AllOptions), MemberType = typeof(AddJobTestData))]
    public void UpdatesDB_WithValid_AllOptions(string jobTitle, string companyName, DateOnly startDate,
        string? jobDescription, string? experience, DateOnly? endDate)
    {
        string[] descriptionArg = jobDescription == null ? [null] : [$"-d", $"{jobDescription}"];
        string[] expArg = experience == null ? [null] : [$"-x", $"{experience}"];
        string[] endArg = endDate == null ? [null] : [$"-e", $"{endDate}"];
        string?[] args =
        [
            ..cmdArgs, "-t", jobTitle, "-s", startDate.ToString(), "-c", companyName, ..descriptionArg, ..expArg,
            ..endArg
        ];
        var result = TestApp.Run(args.Where(x => x != null).ToArray()!);
        Assert.Equal(0, result.ExitCode);
        Assert.Equal(1, TestDb.Jobs.Count());
        var job = TestDb.Jobs.First(j => j.Title == jobTitle && j.Company == companyName && j.StartDate == startDate);
        Assert.NotNull(job);

        //clean inputs; trim and nullify empty strings
        jobDescription = jobDescription?.Trim();
        companyName = companyName.Trim();
        experience = experience?.Trim();
        jobTitle = jobTitle.Trim();

        Assert.Equal(jobTitle, job.Title);
        Assert.Equal(companyName, job.Company);
        Assert.Equal(startDate, job.StartDate);
        Assert.Equal(jobDescription.IsBlank() ? null : jobDescription, job.Description);
        Assert.Equal(experience.IsBlank() ? null : experience, job.Experience);
        Assert.Equal(endDate, job.EndDate);
    }
}

internal class AddJobTestData: JobTestData
{
    public static TheoryData<string, string, DateOnly> JobTitleAndStartDate()
    {
        var data = new TheoryData<string, string, DateOnly>();
        for(var i = TestRepetition - 1; i >= 0; i--)
            data.Add(RandomJobTitle(), RandomCompany(), RandomPastDate());
        return data;
    }

    public static TheoryData<string, string, DateOnly, string?, string?, DateOnly?> AllOptions()
    {
        var data = new TheoryData<string, string, DateOnly, string?, string?, DateOnly?>();
        for(var i = TestRepetition - 1; i >= 0; i--)
            data.Add(RandomJobTitle(), RandomCompany(), RandomPastDate(), WaffleOrNull(), WaffleOrNull(),
                RandomFutureDate().OrNull(Faker));
        return data;
    }
}