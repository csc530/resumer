using Microsoft.EntityFrameworkCore;
using resume_builder.models;

namespace TestResumeBuilder;

public sealed class TestContext: ResumeContext
{
    public TestContext(): base()
    {
        DbPath = "test.db";
        Database.Migrate();
    }

    public void Clear()
    {
        Jobs.RemoveRange(Jobs);
        Projects.RemoveRange(Projects);
        Profiles.RemoveRange(Profiles);
        Companies.RemoveRange(Companies);
        Skills.RemoveRange(Skills);
        SaveChanges();
    }
}