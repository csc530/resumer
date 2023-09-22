using resume_builder;

namespace TestResumeBuilder.test_data;

public static class TestData
{
	public static DateOnly[] Dates =
	{
		DateOnly.MaxValue, DateOnly.MinValue, Today, new(2023, 1, 18),
		new(2002, 9, 0x010), new(0x1c10, 6, 23)
	};

	public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

	public static string[] RelativePaths => new[]
	{
		"newdir", "newdir/withsubdir", ".", "./", "./somelocation/with/a/heck-/loooooooong/path/withsub/dirs/",
		".loooooooong/path/withsub/dirs/", "/i/wonder", "/leading slash", "/"
	};

	//could just implement with Random attribute ([Random()]) for args and then construct a job or whateves in the actual test class/method
}