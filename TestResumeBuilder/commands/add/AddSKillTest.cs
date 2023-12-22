using resume_builder.models;

namespace TestResumeBuilder.commands.add;

public class AddSkillTest: TestBase
{
    private readonly string[] cmdArgs = { "add", "skill" };


    [Theory]
    [MemberData(nameof(AddSkillTestData.GetSkillData), MemberType = typeof(AddSkillTestData))]
    public void AddsSkill_WhenAllFieldsAreValid_ShouldSucceed(string skillName, SkillType skillType)
    {
        var result = TestApp.Run(cmdArgs, skillName, skillType.ToString());
        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void PromptsUser_WhenFieldsAreEmpty_ShouldFail()
    {
        Assert.ThrowsAny<Exception>(() => TestApp.Run(cmdArgs));
    }

    [Theory]
    [InlineData(SkillType.Soft)]
    [InlineData(SkillType.Hard)]
    [InlineData(SkillType.Technical)]
    public void ReturnsError_WhenSkillNameIsEmpty_ShouldFail(SkillType skillType)
    {
        Assert.ThrowsAny<Exception>(() => TestApp.Run(cmdArgs, "", skillType.ToString()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("debugging")]
    [InlineData(
        "r ut erat et ipsum stet sed zzril eos aliquyam sed duo nisl iusto. Clita clita ipsum dolor illum ut ipsum lorem kasd sea dolores rebum et elitr elitr magna. Feugiat takimata amet mi")]
    public void ReturnsError_WhenSkillTypeIsInvalid_ShouldFail(string invalidSkillType)
    {
        Assert.ThrowsAny<Exception>(() => TestApp.Run(cmdArgs, "skill", invalidSkillType));
    }
}

public class AddSkillTestData: TestData
{
    public static TheoryData<string, SkillType> GetSkillData()
    {
        var skillTypes = Enum.GetValues<SkillType>();
        var skillData = new TheoryData<string, SkillType>();
        for(var i = 0; i < TestRepetition; i++)
            skillData.Add(Faker.Random.Words(), skillTypes[i % skillTypes.Length]);
        return skillData;
    }
}