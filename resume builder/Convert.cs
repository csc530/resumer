namespace resume_builder;

public static class Convert
{
	public static int ToInt(this ExitCode exitCode) => (int)exitCode;
	public static DateOnly ToDateOnly(this DateTime date) => DateOnly.FromDateTime(date);

	public static List<string> Prefix(this IEnumerable<string> strings, string txt) => strings.Select(s => txt + s).ToList();
}