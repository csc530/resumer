using resume_builder;
using resume_builder.models;

namespace TestResumeBuilder.commands.get;

[UsesVerify]
public class GetJobTest: TestBase
{
    private static readonly string[] cmdArgs = ["get", "job"];

    [Fact]
    public void GetJobs_WithNoJobData_ShouldReturnNothing()
    {
        //when
        var result = TestApp.Run(cmdArgs);
        //then
        Assert.Empty(TestDb.Jobs);
        Assert.Equal(ExitCode.Success.ToInt(), result.ExitCode);
    }

    [Fact]
    public void GetJobs_WithJobData_ShouldReturnAllJobs()
    {
        //given
        var jobs = JobTestData.RandomJobs(10).ToList();
        TestDb.Jobs.AddRange(jobs);
        TestDb.SaveChanges();
        //when
        var result = TestApp.Run(cmdArgs);
        //then
        Assert.Equal(ExitCode.Success.ToInt(), result.ExitCode);
        for(var index = 0; index < jobs.Count; index++)
            //? the \u2502 is the unicode character for the | character; it's used in the table output
            Assert.Contains("\u2502 " + index, TestConsole.Output);
    }

    [Fact]
    public void GetJob_WithNoArgs_ShouldSucceed()
    {
        //when
        var result = TestApp.Run(cmdArgs);
        //then
        Assert.Equal(ExitCode.Success.ToInt(), result.ExitCode);
    }

    [Fact]
    public void GetJob_WithValidSpecifiedIndicies_ShouldSucceed()
    {
        //given
        TestDb.Jobs.AddRange(JobTestData.RandomJobs(10));
        TestDb.SaveChanges(true);
        //when
        var result = TestApp.Run(cmdArgs, "1", "2", "3");
        //then
        Assert.Equal(ExitCode.Success.ToInt(), result.ExitCode);
        Assert.Multiple(() =>
        {
            Assert.Contains("1", TestConsole.Output);
            Assert.Contains("2", TestConsole.Output);
            Assert.Contains("3", TestConsole.Output);
        });
    }

    [Fact]
    public void GetJob_WithInValidSpecifiedIndicies_ShouldFail()
    {
        //given
        TestDb.Jobs.AddRange(JobTestData.RandomJobs(10));
        TestDb.SaveChanges();
        //when
        var result = TestApp.Run(cmdArgs, "52", "13", "313");
        //then
        Assert.NotEqual(ExitCode.Success.ToInt(), result.ExitCode);
    }
}