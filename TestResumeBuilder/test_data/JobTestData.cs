using resume_builder;
using resume_builder.models;

namespace TestResumeBuilder.test_data;

public static class JobTestData
{
	public static object[][] JobTitleAndStartDates =>
		JobTitles
			.Select<string, object[]>((jobTitle, i) => new object[]
				{ jobTitle, TestData.Dates[i % TestData.Dates.Length] })
			.ToArray();

	public static readonly string[] JobTitles = new[]
	{
		"pretty princess", "villain", "super villain",
		"deputy minister of the united conglomerate for foreign nations new international coast guard protection services",
		"executive assistant", "doctor", "developer", "student", "lead executive office manager", "clothing cashier"
	};

	public static readonly string[] Companies = { "walmart", "Asus", "The super bros. Inc.", "Yallreadyknow", };

	public static (DateOnly startDate, DateOnly endDate)[] StartAndEndDates => new[]
	{
		(TestData.Today, TestData.Today.AddDays(10)), (TestData.Today.AddMonths(-1), TestData.Today),
		(TestData.Today.AddYears(-12), TestData.Today.AddMonths(-43)),
		(new DateOnly(2012, 12, 31), new DateOnly(2014, 4, 24)), (DateOnly.MinValue, DateOnly.MaxValue)
	};

	public static Job[] Jobs =
	{
		new("developer", Globals.Today, company: "Plexxis", description: "writing code and improving features",
			experience: "never hired and always rejected before an interview🥲"),
		new("janitor", new(2012, 01, 13), new(2015, 06, 30), "Cineplex",
			"maintain a healthy and clean facilities\nensure the proper working order of concession stands and machines",
			"mop puke and clean popcorn from theatres"),
		new("executive coordinator of consumer relations", new(2018, 09, 1), new(2019, 02, 04), "Twitter",
			"manage communications and development teams\ncoordinate PR campaigns and promotions",
			"send tweets and troll")
	};
}