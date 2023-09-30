using System.Data.Common;
using Microsoft.Data.Sqlite;
using resume_builder.models;
using Spectre.Console;

namespace resume_builder;

public static class Globals
{
	public static DateOnly Today { get; } = DateOnly.FromDateTime(DateTime.Today);
	public const string NullString = "<null/>";
}

public static class Extensions
{
	public static int ToInt(this ExitCode exitCode) => (int)exitCode;
	public static DateOnly ToDateOnly(this DateTime date) => DateOnly.FromDateTime(date);

	public static string Prefix(this string s, string txt) => $"{txt}{s}";

	public static List<string> Prefix(this IEnumerable<string> strings, string txt) =>
		strings.Select(s => s.Prefix(txt)).ToList();

	public static string Surround(this string s, string txt) => $"{txt}{s}{txt}";

	public static List<string> Surround(this IEnumerable<string> strings, string txt) =>
		strings.Select(s => s.Surround(txt)).ToList();

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

	public static T? GetNullableValue<T>(this DbDataReader reader, int ordinal)
	{
		var val = GetNullableValue(reader, ordinal);
		return val == null ? (T?)val : reader.GetFieldValue<T?>(ordinal);
	}

	/// <summary>
	/// because why won't it just auto convert it to a <see cref="DBNull.Value"/>
	/// </summary>
	/// <inheritdoc cref="SqliteParameterCollection.AddWithValue(string,object)"/>
	public static void AddWithNullableValue(this SqliteParameterCollection parameters, string name, object? value)
		=> parameters.AddWithValue(name, value ?? DBNull.Value);

	public static void BindParameters(this SqliteCommand cmd, Dictionary<string, object?> placeholderValuePairs)
	{
		foreach(var (placeholder, value) in placeholderValuePairs)
			cmd.Parameters.AddWithNullableValue(placeholder, value);
	}

	public static void BindParameters(this SqliteCommand cmd,
	                                  IEnumerable<KeyValuePair<string, object?>> placeholderValuePairs)
	{
		foreach(var (placeholder, value) in placeholderValuePairs)
			cmd.Parameters.AddWithNullableValue(placeholder, value);
	}

	public static TextPrompt<T> Clone<T>(this TextPrompt<T> textPrompt, string prompt, T defaultValue = default) =>
		new TextPrompt<T>(prompt)
		{
			ShowDefaultValue = textPrompt.ShowDefaultValue,
			AllowEmpty = textPrompt.AllowEmpty,
			Validator = textPrompt.Validator,
			ValidationErrorMessage = textPrompt.ValidationErrorMessage,
		}.DefaultValue(defaultValue);
	//todo: inquire about default value being a property
	// .DefaultValue(textPrompt);
}