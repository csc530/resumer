using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace resume_builder.models.database.query;

/// <summary>
/// Create a SqliteCommand from an object
/// </summary>
public static class Query
{
	/// <summary>creates a command to insert an object into a table</summary>
	///<remarks>inserts the object into a table that matches the object's <see cref="TableAttribute"/></remarks>
	/// <param name="obj">The object to add</param>\
	/// <param name="connection">The connection to the database</param>
	///
	/// <returns> A <see cref="SqliteCommand"/> object.</returns>
	public static SqliteCommand CreateAddCommand(this SqliteConnection connection, object obj)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.Add);
	}

	public static SqliteCommand CreateUpdateCommand(this SqliteConnection connection, object obj,
	                                                KeyValuePair<string, object?>? idColumnNameValuePair = null)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.Update, idColumnNameValuePair);
	}

	public static SqliteCommand CreateDeleteCommand(this SqliteConnection connection, object obj,
	                                                KeyValuePair<string, object?>? idColumnNameValuePair = null)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.Delete, idColumnNameValuePair);
	}

	public static SqliteCommand CreateDeleteAllCommand(this SqliteConnection connection, object obj)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.DeleteAll);
	}

	/// <summary> The DropTable function creates a SqliteCommand from this <see cref="connection"/> that drops the table associated with the given object.</summary>
	///
	/// <param name="connection">
	/// the connection to the database.
	/// </param>
	/// <param name="obj">the object to be used as the basis for the sql command.
	/// </param>
	///
	/// <returns> A prepared <see cref="SqliteCommand"/></returns>
	public static SqliteCommand CreateDropTableCommand(this SqliteConnection connection, object obj) =>
		CreateSqlCommand(connection, obj, SqlOperation.Drop);

	public static SqliteCommand CreateDropTableIfExistsCommand(this SqliteConnection connection, object obj)
		=> CreateSqlCommand(connection, obj, SqlOperation.DropIfExists);

	public static SqliteCommand CreateSelectAllCommand(this SqliteConnection connection, object obj)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.SelectAll);
	}

	public static SqliteCommand CreateSelectAllCommand<T>(this SqliteConnection connection)
	{
		return CreateSqlCommand<T>(connection, SqlOperation.SelectAll);
	}

	private static SqliteCommand CreateSqlCommand<T>(SqliteConnection connection, SqlOperation sqlOperation,
	                                                 Dictionary<string, object?>? idColumnNameValuePairs = null,
	                                                 params string[] selectColumns)
	{
		var tableName = typeof(T).GetTableName();
		if(tableName == null)
			throw new ArgumentException("object does not have a table in the database", nameof(T));


		var cmdTxt = CreateCommandText(columnNames: Array.Empty<string>(), sqlOperation, selectColumns)
			.Replace("$table", tableName);
		var cmd = new SqliteCommand(cmdTxt, connection);

		// * can't use parameter for columns/tables or identifiers
		// because when they are inserted they are warped to inputs and  not just raw identifiers
		// cmd.Parameters.AddWithValue("$table", tableName);
		if(idColumnNameValuePairs != null)
			foreach(var (name, value) in idColumnNameValuePairs)
			{
				// * but is it safe sql to just inject the column and table names?
				// cmd.Parameters.AddWithValue($":{name}", name);
				cmd.CommandText = cmd.CommandText.Replace($":{name}", name);
				cmd.Parameters.AddWithNullableValue($"@{name}", value);
			}

		cmd.Prepare();
		return cmd;
	}

	public static SqliteCommand CreateSelectCommand(this SqliteConnection connection, object obj,
	                                                params string[] columns)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.Select, null, columns);
	}

	private static SqliteCommand CreateSqlCommand<T>(SqliteConnection? connection, T obj, SqlOperation sqlOperation,
	                                                 KeyValuePair<string, object?>? idColumnNameValuePair = null,
	                                                 params string[] selectColumns)
	{
		var tableName = obj.GetTableName();
		if(tableName == null)
			throw new ArgumentException("object does not have a table in the database", nameof(obj));

		var columnNameValuePairs = obj.GetPropertyColumnNameValuePairs();
		var columnNames = columnNameValuePairs.Keys.ToList();
		// ? add id column to columnNames list so it's included in the statements where clause
		if(idColumnNameValuePair != null)
			columnNames.Add(idColumnNameValuePair.Value.Key);

		var cmdTxt = CreateCommandText(columnNames, sqlOperation, selectColumns)
			.Replace("$table", tableName);
		var cmd = new SqliteCommand(cmdTxt, connection);

		// * can't use parameter for columns/tables or identifiers
		// because when they are inserted they are warped to inputs and  not just raw identifiers
		// cmd.Parameters.AddWithValue("$table", tableName);
		foreach(var (name, value) in columnNameValuePairs)
		{
			// * but is it safe sql to just inject the column and table names?
			// cmd.Parameters.AddWithValue($":{name}", name);
			cmd.CommandText = cmd.CommandText.Replace($":{name}", name);
			cmd.Parameters.AddWithNullableValue($"@{name}", value);
		}

		if(idColumnNameValuePair != null)
		{
			cmd.Parameters.AddWithValue($":{idColumnNameValuePair.Value.Key}", idColumnNameValuePair.Value.Key);
			cmd.Parameters.AddWithNullableValue($"@{idColumnNameValuePair.Value.Value}",
				idColumnNameValuePair.Value.Value);
		}

		cmd.Prepare();
		return cmd;
	}

	private static string CreateCommandText(IReadOnlyCollection<string> columnNames, SqlOperation sqlOperation,
	                                        params string[] selectColumns)
	{
		var condition = string.Join(" AND ", columnNames.Select(name => $":{name} IS @{name}"));
		switch(sqlOperation)
		{
			case SqlOperation.Add:
				var columnValuePlaceholders = string.Join(", ", columnNames.Select(name => "@" + name));
				var columnNamePlaceholders = string.Join(", ", columnNames.Select(name => $":{name}"));
				return $"INSERT INTO $table ({columnNamePlaceholders}) VALUES ({columnValuePlaceholders});";
			case SqlOperation.Update:
				var updateColumns = string.Join(", ", columnNames.Select(name => $":{name} = @{name}"));
				return $"UPDATE $table SET {updateColumns} WHERE {condition};";
			case SqlOperation.UpdateAll:
				updateColumns = string.Join(", ", columnNames.Select(name => $":{name} = @{name}"));
				return $"UPDATE $table SET {updateColumns};";
			case SqlOperation.Delete:
				return $"DELETE FROM $table WHERE {condition};";
			case SqlOperation.DeleteAll:
				return "DELETE FROM $table;";
			case SqlOperation.Select:
				return selectColumns.Length == 0
					? $"SELECT * FROM $table WHERE {condition};"
					: $"SELECT {string.Join(", ", selectColumns)} FROM $table WHERE {condition};";
			case SqlOperation.SelectAll:
				return selectColumns.Length == 0
					? "SELECT * FROM $table;"
					: $"SELECT {string.Join(", ", selectColumns)} FROM $table;";
			case SqlOperation.Drop:
				return "DROP TABLE $table;";
			case SqlOperation.DropIfExists:
				return "DROP TABLE IF EXISTS $table;";
			default:
				throw new ArgumentOutOfRangeException(nameof(sqlOperation), sqlOperation, null);
		}
	}

	#region extensions

	/// <summary> The GetPropertyValue function returns the value of a property on an object.</summary>
	///
	/// <param name="obj"> The object to get the property value from.</param>
	/// <param name="propertyName"> The name of the property to get.</param>
	///
	/// <returns> The value of the property specified by the <see cref="propertyName"/> parameter. if no such property exists, it returns null.</returns>
	private static object? GetPropertyValue(this object obj, string propertyName)
	{
		return obj.GetType()
		          .GetProperty(propertyName)
		          ?.GetValue(obj);
	}

	#region class sql attributes

	/// <summary>
	/// return the related table name of the object's class
	/// </summary>
	/// <param name="obj">the object to get the table name</param>
	/// <returns>return <see cref="TableAttribute"/>'s name from object of the object</returns>
	private static string? GetTableName<T>(this T obj) => typeof(T).GetTableName();

	private static string? GetTableName(this Type type) => type.GetCustomAttribute<TableAttribute>()?.Name;

	/// <summary>
	/// return the related column name of the given property
	/// </summary>
	/// <param name="obj">the object to get the column name</param>
	/// <param name="propertyName">the object's property name to get the column name</param>
	/// <returns>return <see cref="ColumnAttribute"/>'s name from object of the object</returns>
	private static string? GetColumnName(this object obj, string propertyName) =>
		obj.GetType()
		   .GetProperty(propertyName)
		   ?.GetCustomAttribute<ColumnAttribute>()
		   ?.Name;

	/// <summary>
	/// return the all non-null column names of the given object's properties
	/// </summary>
	/// <param name="obj">the object to get the column names</param>
	/// <returns>return a list of <see cref="ColumnAttribute"/>'s <see cref="ColumnAttribute.Name"/></returns>
	private static List<string> GetColumnNames(this object obj) =>
		obj.GetType()
		   .GetProperties()
		   .Select(property => property.GetCustomAttribute<ColumnAttribute>()?.Name)
		   .Where(name => name != null)
		   .ToList()!;

	/// <summary>
	/// return a dictionary of the given object's properties and their values.
	/// <para>
	/// where the key is the property name and the value is the property value
	/// </para>
	/// </summary>
	/// <param name="obj">the object to construct its property name and value dictionary</param>
	/// <returns>return a dictionary of the given object's properties and their values</returns>
	private static Dictionary<string, object?> GetPropertyNameValuePairs(this object obj) =>
		obj.GetType()
		   .GetProperties()
		   .ToDictionary(property => property.Name, property => property.GetValue(obj));

	/// <summary>
	/// creates a dictionary of the given object's properties <see cref="ColumnAttribute"/>'s <see cref="ColumnAttribute.Name"/>
	/// and values
	/// </summary>
	///
	/// <param name="obj"> The object to get the property's column name and value pairs from.</param>
	///
	/// <returns> A dictionary of column names and values.</returns>
	private static Dictionary<string, object?> GetPropertyColumnNameValuePairs(this object obj)
	{
		return obj.GetPropertyNameValuePairs()
		          .Where(pair => obj.GetColumnName(pair.Key) != null)
		          .ToDictionary(pair => obj.GetColumnName(pair.Key)!, pair => pair.Value);
	}

	#endregion

	#endregion

	private enum SqlOperation
	{
		Select,
		SelectAll,
		Add,
		Update,
		UpdateAll,
		Delete,
		DeleteAll,
		Drop,
		DropIfExists
	}
}