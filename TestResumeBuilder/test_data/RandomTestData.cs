using NUnit.Framework.Internal;
using resume_builder;

namespace TestResumeBuilder.test_data;

public static class RandomTestData
{
	private static readonly Randomizer Randomizer = new((int)DateTime.UtcNow.Ticks);

	public static DateOnly GetRandomDate =>
		new(Randomizer.Next(1, 10000), Randomizer.Next(1, 13), Randomizer.Next(1, 30));

	public static string[] RandomStrings()
	{
		var list = new List<string>();
		for(int i = 0; i < Randomizer.Next(100); i++)
			list.Add(GetRandomString(101));
		return list.ToArray();
	}

	public static string GetRandomString(int length = -1) =>
		Randomizer.GetString(length < 0 ? Randomizer.Next() : Randomizer.Next(length));

	public static List<DateOnly> RandomDates()
	{
		var dates = new List<DateOnly>();
		for(var i = 0; i < Randomizer.Next(100); i++)
			dates.Add(new DateOnly(Randomizer.Next(1, 1000), Randomizer.Next(1, 13), Randomizer.Next(1, 31)));
		return dates;
	}

	public static Job GetRandomJob()
	{
		var date = GetRandomDate;
		return new Job(Randomizer.GetString(), date,
			Randomizer.NextBool() ? date.AddDays(Randomizer.Next(10)) : null,
			Randomizer.NextBool() ? Randomizer.GetString() : null,
			Randomizer.NextBool() ? Randomizer.GetString() : null,
			Randomizer.NextBool() ? Randomizer.GetString() : null);
	}
}