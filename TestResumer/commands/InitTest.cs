namespace TestResumer.commands;

public class InitTest : TestBase
{
    private const string CmdArgs = "init";

    [Fact]
    public void Init_WithNoArgs_ShouldReturnSuccess()
    {

        //when
        var result = TestApp.Run(CmdArgs);
        //then
        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Init_WithNoArgs_ShouldCreateDb()
    {
        //when
        var result = TestApp.Run(CmdArgs);
        //then
        Assert.Equal(0, result.ExitCode);
        Assert.True(File.Exists("resume.db"));
        Assert.True(TestDb.Database.CanConnect());
    }
}