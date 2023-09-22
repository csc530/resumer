using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace resume_builder;

public static class Convert
{
	public static int ToInt(this ExitCode exitCode) => (int)exitCode;
	public static DateOnly ToDateOnly(this DateTime date) => DateOnly.FromDateTime(date);

	public static List<string> Prefix(this IEnumerable<string> strings, string txt) =>
		strings.Select(s => txt + s).ToList();

	public static object? GetNullableValue(this DbDataReader reader, string columnName)
	{
		var ordinal = reader.GetOrdinal(columnName);
		return reader.GetNullableValue(ordinal);
	}

	public static object? GetNullableValue(this DbDataReader reader, int ordinal) =>
		reader.IsDBNull(ordinal) ? null : reader[ordinal];

	public static T? GetNullableValue<T>(this DbDataReader reader, string columnName)
	{
		var ordinal = reader.GetOrdinal(columnName);
		return reader.GetNullableValue<T>(ordinal);
	}

	public static T? GetNullableValue<T>(this DbDataReader reader, int ordinal) =>
		(T?)GetNullableValue(reader, ordinal);

	/// <summary>
	/// because why won't it just auto convert it to a <see cref="DBNull.Value"/>
	/// </summary>
	/// <inheritdoc cref="SqliteParameterCollection.AddWithValue(string,object)"/>
	public static void AddWithNullableValue(this SqliteParameterCollection parameters, string name, object? value)
		=> parameters.AddWithValue(name, value ?? DBNull.Value);
}