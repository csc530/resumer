namespace TestResumeBuilder.test_data;

public static class AddJobTestData
{
	public static object[][] GetRequiredArgs()
	{
		var arr = Array.Empty<object[]>();
		for(int i = 0; i < TestData.Jobtitles.Length; i++)
			arr = arr.Append(new object[] { TestData.Jobtitles[i], TestData.Dates[i % TestData.Dates.Length] })
			         .ToArray();
		return arr;
	}
}