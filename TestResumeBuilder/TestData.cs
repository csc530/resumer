using System.Runtime.InteropServices.JavaScript;
using NUnit.Framework.Internal;
using resume_builder;

namespace TestResumeBuilder;

public static class TestData
{
	public static string[] Jobtitles = new[]
	{
		"pretty princess", "villain", "super villain",
		"deputy minister of the united conglomerate for foreign nations new international coast guard protection services",
		"executive assistant", "doctor"
	};

	public static DateOnly[] Dates = new[]
	{
		DateOnly.MaxValue, DateOnly.MinValue, Today, new(2023, 1, 18),
		new(2002, 9, 0x010), new(0x1c10, 6, 23)
	};

	public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

	public static string[] Companies = new[] { "walmart", "Asus", "The super bros. Inc.", "Yallreadyknow", };

	public static (DateOnly startDate, DateOnly endDate)[] StartAndEndDates => new[]
	{
		(Today, Today.AddDays(10)), (Today.AddMonths(-1), Today), (Today.AddYears(-12), Today.AddMonths(-43)),
		(new DateOnly(2012, 12, 31), new DateOnly(2014, 4, 24)), (DateOnly.MinValue, DateOnly.MaxValue)
	};

	public static string[] Paths => new[]
	{
		"newdir", "newdir/withsubdir", ".", "./", "./somelocation/with/a/heck-/loooooooong/path/withsub/dirs/",
		".loooooooong/path/withsub/dirs/", "/i/wonder", "/leading slash", "/"
	};

	//could just implement with Random attribute ([Random()]) for args and then construct a job or whateves in the actual test class/method


	public static Job[] Jobs()
	{
		var list = new List<Job>();
		for(int i = 0; i < Jobtitles.Length; i++)
		{
			var (start, end) = StartAndEndDates[i % StartAndEndDates.Length];
			var job = new Job(Jobtitles[i], (start, end).start, end, Companies[i % Companies.Length]);
			list.Add(job);
		}

		return list.ToArray();
	}

	public static class RandomData
	{
		private static Randomizer Randomizer = new(((int)DateTime.UtcNow.Ticks));

		public static DateOnly RandomDate =>
			new(Randomizer.Next(1, 1000), Randomizer.Next(1, 13), Randomizer.Next(1, 31));
	}
}