namespace resume_builder;

public static class Convert
{
	public static int ToInt(this ExitCode exitCode) => (int)exitCode;
	public static DateOnly ToDateOnly(this DateTime date) => DateOnly.FromDateTime(date);

	public static List<string> Prefix(this IEnumerable<string> strings, string txt) =>
		strings.Select(s => txt + s).ToList();

	public static Dictionary<T, TS> CreateDictionary<T, TS>(IEnumerable<T> keys, IEnumerable<TS> values)
		where T : notnull
	{
		var dic = new Dictionary<T, TS>();
		foreach(var (key, value) in keys.Zip(values))
			dic.Add(key, value);
		return dic;
	}
}