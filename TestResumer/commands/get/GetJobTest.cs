using Microsoft.EntityFrameworkCore;
using Resumer.models;
using TestResumer.data;

namespace TestResumer.commands.get;

public class GetJobTest : TestBase
{
    public GetJobTest()
    {
        TestDb.Database.Migrate();
    }

    private static readonly string[] CmdArgs = ["get", "job"];

    [Fact]
    public void GetJobs_WithNoJobData_ShouldReturnNothing()
    {
        //when
        var result = TestApp.Run(CmdArgs);
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
        var result = TestApp.Run(CmdArgs);
        //then
        Assert.Equal(ExitCode.Success.ToInt(), result.ExitCode);
        for(var index = 0; index < jobs.Count; index++)
            //? the \u2502 is the unicode character for the | character; it's used in the table output
            Assert.Contains(index.ToString(), TestConsole.Output);
    }

    [Fact]
    public void GetJob_WithNoArgs_ShouldSucceed()
    {
        //when
        var result = TestApp.Run(CmdArgs);
        //then
        Assert.Equal(ExitCode.Success.ToInt(), result.ExitCode);
    }
}