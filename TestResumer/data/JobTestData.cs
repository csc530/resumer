using Bogus;
using Resumer.models;

namespace TestResumer.data;

internal class JobTestData: TestData
{
    private static Faker<Job> BogusJob { get; set; } = new Faker<Job>()
                                                      .RuleFor(job => job.Title, RandomJobTitle)
                                                      .RuleFor(job => job.Description, RandomTextOrNull)
                                                      .RuleFor(job => job.Experience, RandomTextOrNull)
                                                      .RuleFor(job => job.Company, RandomCompany)
                                                      .RuleFor(job => job.StartDate, RandomPastDate)
                                                      .RuleFor(job => job.EndDate, RandomEndDate);

    public static string RandomJobTitle() => Faker.Name.JobTitle();
    public static string RandomCompany() => Faker.Company.CompanyName();
    public static DateOnly? RandomEndDate() => RandomPastDate().OrNull(Faker);
    public static IEnumerable<Job> EternalJobs() => BogusJob.GenerateForever();
    public static Job RandomJob() => BogusJob.Generate();
    public static IEnumerable<Job> RandomJobs(int count = TestRepetition) => BogusJob.GenerateLazy(count);
}