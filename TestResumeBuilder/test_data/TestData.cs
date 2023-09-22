using resume_builder;

namespace TestResumeBuilder.test_data;

public static class TestData
{
	public static readonly string[] JobTitles = new[]
	{
		"pretty princess", "villain", "super villain",
		"deputy minister of the united conglomerate for foreign nations new international coast guard protection services",
		"executive assistant", "doctor", "developer", "student", "lead executive office manager", "clothing cashier"
	};

	public static DateOnly[] Dates = new[]
	{
		DateOnly.MaxValue, DateOnly.MinValue, Today, new(2023, 1, 18),
		new(2002, 9, 0x010), new(0x1c10, 6, 23)
	};

	public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

	public static string[] Companies = { "walmart", "Asus", "The super bros. Inc.", "Yallreadyknow", };

	public static (DateOnly startDate, DateOnly endDate)[] StartAndEndDates => new[]
	{
		(Today, Today.AddDays(10)), (Today.AddMonths(-1), Today), (Today.AddYears(-12), Today.AddMonths(-43)),
		(new DateOnly(2012, 12, 31), new DateOnly(2014, 4, 24)), (DateOnly.MinValue, DateOnly.MaxValue)
	};

	public static string[] RelativePaths => new[]
	{
		"newdir", "newdir/withsubdir", ".", "./", "./somelocation/with/a/heck-/loooooooong/path/withsub/dirs/",
		".loooooooong/path/withsub/dirs/", "/i/wonder", "/leading slash", "/"
	};

	//could just implement with Random attribute ([Random()]) for args and then construct a job or whateves in the actual test class/method


	public static List<Job> Jobs()
	{
		var list = new List<Job>();
		for(int i = 0; i < JobTitles.Length; i++)
		{
			var (start, end) = StartAndEndDates[i % StartAndEndDates.Length];
			var job = new Job(JobTitles[i], (start, end).start, end, Companies[i % Companies.Length]);
			list.Add(job);
		}

		return list;
	}
}