using Resumer.models;

namespace TestResumer.commands;

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
        Assert.Equal(0, result.ExitCode);
        Assert.True(File.Exists("resume.db"));
        TestDb = new ResumeContext();
        Assert.True(TestDb.Database.CanConnect());
    }
}