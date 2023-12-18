using resume_builder.models;

namespace TestResumeBuilder.commands;

public class InitTest: TestBase
{
    [Fact]
    public void Init_WithNoArgs_ShouldReturnSuccess()
    {
        //when
        var result = TestApp.Run("init");
        //then
        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Init_WithNoArgs_ShouldCreateDb()
    {
        //when
        var result = TestApp.Run("init");
        //then
        Assert.True(File.Exists("TestResumeBuilder.db"));
        Assert.Equal(0, result.ExitCode);
        TestDb = new ResumeContext();
        Assert.True(TestDb.Database.CanConnect());
    }
}