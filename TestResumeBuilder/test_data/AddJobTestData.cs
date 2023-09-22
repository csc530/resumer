namespace TestResumeBuilder.test_data;

public static class AddJobTestData
{
	public static object[][] GetRequiredArgs() =>
		TestData.JobTitles
		        .Select((jobTitle, i) => new object[]
			        { jobTitle, TestData.Dates[i % TestData.Dates.Length] })
		        .ToArray();
}