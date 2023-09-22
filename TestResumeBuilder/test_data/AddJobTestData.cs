namespace TestResumeBuilder.test_data;

public static class AddJobTestData
{
	public static object[][] JobTitleAndStartDates() =>
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
}