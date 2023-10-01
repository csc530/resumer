using System.Data.Common;
using Microsoft.Data.Sqlite;
using resume_builder.cli.settings;
using resume_builder.models;
using Spectre.Console;

namespace resume_builder;

public static class Globals
{
	public static DateOnly Today { get; } = DateOnly.FromDateTime(DateTime.Today);
	public const string NullString = "<null/>";

	public static int PrintError(CLISettings settings, Exception exception)
	{
		AnsiConsole.Foreground = Color.Red;
		if(exception.GetType() == typeof(SqliteException))
		{
			var e = (SqliteException)exception;
			var err = $"Database Error {e.SqliteErrorCode}";
			if(settings.Verbose)
				err += $"-{e.SqliteExtendedErrorCode}";
			err += $": {((SQLResultCode)e.SqliteErrorCode).GetMessage()}";
			AnsiConsole.WriteLine($"{err}");
			if(settings.Verbose)
				AnsiConsole.WriteLine($"{e.Message}");
			return ExitCode.DbError.ToInt();
		}
		else if(exception is InvalidOperationException)
		{
			AnsiConsole.WriteLine("Invalid operation");
			return ExitCode.Fail.ToInt();
		}
		else
		{
			AnsiConsole.WriteLine(exception.Message);
			return ExitCode.Error.ToInt();
		}
	}
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
	//todo: inquire about default value being a property - spectre console pr/iss
	// .DefaultValue(textPrompt);

	public static string GetMessage(this SQLResultCode code) => code switch
	{
		SQLResultCode.Success => "Success",
		SQLResultCode.Error => "Error",
		SQLResultCode.Readonly => "Database is readonly",
		SQLResultCode.IOErr => "disk I/O error occurred",
		SQLResultCode.NotNull => "not null constraint violated",
		SQLResultCode.Abort => "Operation terminated by interrupt (sqlite3_interrupt)",
		SQLResultCode.Constraint => "constraint violation",
		_ => "Unknown error"
	};

	public static string GetPrintValue(this string? value, bool allowBlank = false)
	{
		if(allowBlank)
			return value ?? "";
		return string.IsNullOrWhiteSpace(value) ? Globals.NullString : value;
	}

	public static string GetPrintValue(this object? value, bool allowBlank = false) =>
		GetPrintValue(value?.ToString(), allowBlank);

	#region Table

	/// <summary>
	/// add column values to the table based on conditions
	/// </summary>
	/// <param name="table">the table to append column values</param>
	/// <param name="columns">a key-value pair of column condition and column values; if the column condition is true, the column value will be added to the table.
	/// if the column condition is false, the column (value) will be skipped</param>
	public static Table AddTableRow(this Table table, params KeyValuePair<bool, object?>[] columns)
	{
		var row = columns.Where(col => col.Key)
		                 .Select(col => col.Value.GetPrintValue(allowBlank: true))
		                 .ToArray();
		table.AddRow(row);
		return table;
	}

	public static Table? AddTableColumn(this Table table, string name, bool nowrap = false) =>
		table.AddColumn(RenderableFactory.CreateTableColumn(name, nowrap));

	public static Table? AddTableColumn(this Table table, bool nowrap = false, params string[] columns)
	{
		foreach(var column in columns)
			table.AddTableColumn(column, nowrap);
		return table;
	}

	#endregion
}

public static class RenderableFactory
{
	public static TextPrompt<T?> CreateText<T>(string prompt, T? defaultValue = default,
	                                           Func<T?, ValidationResult>? validator = null, bool allowEmpty = false,
	                                           string? errorMessage = null) => new TextPrompt<T?>(prompt)
		{
			ShowDefaultValue = defaultValue != null,
			AllowEmpty = defaultValue != null,
			ValidationErrorMessage =
				errorMessage ?? (defaultValue == null ? $"Invalid {prompt}" : $"{prompt} cannot be empty"),
			Validator = validator,
		}
		.DefaultValue(defaultValue);

	public static TableColumn CreateTableColumn(string name, bool nowrap = false) => new TableColumn(name)
	{
		Footer = new Text(name),
		Header = new Text(name),
		NoWrap = nowrap
	};
}