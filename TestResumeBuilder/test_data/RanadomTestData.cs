using NUnit.Framework.Internal;

namespace TestResumeBuilder.test_data;

public static class RanadomTestData
{
	private static readonly Randomizer Randomizer = new((int)DateTime.UtcNow.Ticks);

	public static DateOnly RandomDate =>
		new(Randomizer.Next(1, 1000), Randomizer.Next(1, 13), Randomizer.Next(1, 31));

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
}