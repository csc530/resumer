using Bogus;
using Resumer;
using Resumer.cli.commands.add;
using Resumer.models;
using TestResumer.data;
using WaffleGenerator;

namespace TestResumer.commands.add;

public class AddProjectTest: TestBase
{
    static readonly string[] CmdArgs = {"add", "project"};
    
    [Fact]
    public void ReturnsSuccess_WithNoArgs_AndEnteredPrompts()
    {
        var result = TestApp.Run(CmdArgs);
        Assert.Equal(ExitCode.Error.ToInt(), result.ExitCode);
    }
    
    [Theory]
    [InlineData("projectName")]
    [MemberData(nameof(AddProjectTestData.ProjectNames), MemberType = typeof(AddProjectTestData))]
    public void ReturnsSuccess_WithValid_Name(string projectName)
    {
        var result = TestApp.Run(CmdArgs, "-n", projectName);
        var resultSettings = result.Settings as AddProjectSettings;
        Assert.Multiple(() => {
            Assert.Equal(0, result.ExitCode);
            Assert.NotNull(resultSettings);
            Assert.Equal(projectName, resultSettings.ProjectName);
        });
    }

    [Theory]
    [MemberData(nameof(AddProjectTestData.AllOptions), MemberType = typeof(AddProjectTestData))]
    public void ReturnsSuccess_WithValid_AllOptions(Project project)
    {
        //given
        var args = CreateCmdOptions(project);
        //when
        var result = TestApp.Run(CmdArgs, args.ToArray());
        var resultSettings = result.Settings as AddProjectSettings;
        //then
        Assert.Multiple(() => {
            Assert.Equal(0, result.ExitCode);
            Assert.NotNull(resultSettings);
            Assert.Equal(project.Name, resultSettings.ProjectName);
            Assert.Equal(project.Type, resultSettings.ProjectType);
            Assert.Equal(project.Description, resultSettings.ProjectDescription);
            Assert.Equal(project.Link, resultSettings.ProjectUrl);
            Assert.Equal(project.StartDate, resultSettings.ProjectStartDate);
            Assert.Equal(project.EndDate, resultSettings.ProjectEndDate);
        });
    }

    private static IEnumerable<string> CreateCmdOptions(Project project)
    {
        var args = new List<string>
        {
            "-n",
            project.Name
        };
        if(project.Type != null)
        {
            args.Add("-t");
            args.Add(project.Type);
        }
        if(project.Description != null)
        {
            args.Add("-d");
            args.Add(project.Description);
        }
        if(project.Link != null)
        {
            args.Add("-l");
            args.Add(project.Link.ToString());
        }
        if(project.StartDate != null)
        {
            args.Add("-s");
            args.Add(project.StartDate.ToString()!);
        }
        if(project.EndDate != null)
        {
            args.Add("-e");
            args.Add(project.EndDate.ToString()!);
        }
        if(project.Link != null)
        {
            args.Add("-u");
            args.Add(project.Link.ToString()!);
        }
        return args.ToArray();
    }

    [Fact]
    public void ReturnsError_WithoutProjectName()
    {
        Assert.ThrowsAny<Exception>(() => TestApp.Run(CmdArgs));
        Assert.ThrowsAny<Exception>(() => TestApp.Run(CmdArgs, "-n"));
    }

    [Theory]
    [InlineData("")]
    // [MemberData(nameof(TestData.RandomWhiteSpaceString), MemberType = typeof(TestData))]
    public void ReturnsError_WhenProjectNameIsInvalid(string invalidProjectName)
    {
        Assert.ThrowsAny<Exception>(() => TestApp.Run(CmdArgs, "-n", invalidProjectName));
    }
    
}

public class AddProjectTestData: TestData
{
    public static TheoryData<string> ProjectNames()
    {
        var projectNames = new TheoryData<string>();
        for(var i = 0; i < TestRepetition; i++)
            projectNames.Add(WaffleEngine.Title());
        return projectNames;
    }

    public static TheoryData<Project> AllOptions()
    {
        var projects = new TheoryData<Project>();
        for(var i = 0; i < TestRepetition; i++)
        {
            projects.Add(new Project(Faker.Random.Words())
            {
                Type = Faker.Random.Words(),
                Description = Faker.Random.Words(),
                Link = new Uri(Faker.Internet.Url()),
                StartDate = RandomPastDate().OrNull(Faker),
                EndDate = RandomFutureDate().OrNull(Faker)
            });
        }
        return projects;
    }
}