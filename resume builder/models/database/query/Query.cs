using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace resume_builder.models.database.query;

/// <summary>
///     Create a SqliteCommand from an object
/// </summary>
public static class Query
{
	/// <summary>creates a command to insert an object into a table</summary>
	/// <remarks>inserts the object into a table that matches the object's <see cref="TableAttribute" /></remarks>
	/// <param name="obj">The object to add</param>
	/// \
	/// <param name="connection">The connection to the database</param>
	/// <returns> A <see cref="SqliteCommand" /> object.</returns>
	public static SqliteCommand CreateAddCommand(this SqliteConnection connection, object obj)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.Add);
	}

	// public static SqliteCommand CreateUpdateCommand(this SqliteConnection connection, object obj,
	//                                                 Dictionary<string, object?>? idColumnNameValuePair = null)
	// {
	// 	return CreateSqlCommand(connection, obj, SqlOperation.Update, idColumnNameValuePair);
	// }
	// public static SqliteCommand CreateUpdateCommand(this SqliteConnection connection, object obj,
	//                                                 KeyValuePair<string, object?>? idColumnNameValuePair = null)
	// {
	// 	return CreateSqlCommand(connection, obj, SqlOperation.Update, idColumnNameValuePair);
	// }
	//
	// public static SqliteCommand CreateDeleteCommand(this SqliteConnection connection, object obj,
	//                                                 KeyValuePair<string, object?>? idColumnNameValuePair = null)
	// {
	// 	return CreateSqlCommand(connection, obj, SqlOperation.Delete, idColumnNameValuePair);
	// }

	public static SqliteCommand CreateDeleteAllCommand(this SqliteConnection connection, object obj)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.DeleteAll);
	}

	/// <summary>
	///     The DropTable function creates a SqliteCommand from this <see cref="connection" /> that drops the table
	///     associated with the given object.
	/// </summary>
	/// <param name="connection">
	///     the connection to the database.
	/// </param>
	/// <param name="obj">
	///     the object to be used as the basis for the sql command.
	/// </param>
	/// <returns> A prepared <see cref="SqliteCommand" /></returns>
	public static SqliteCommand CreateDropTableCommand(this SqliteConnection connection, object obj)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.Drop);
	}

	public static SqliteCommand CreateDropTableIfExistsCommand(this SqliteConnection connection, object obj)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.DropIfExists);
	}

	public static SqliteCommand CreateSelectAllCommand(this SqliteConnection connection, object obj)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.SelectAll);
	}

	public static SqliteCommand CreateSelectAllCommand<T>(this SqliteConnection connection)
	{
		return CreateSqlCommand<T>(connection, SqlOperation.SelectAll);
	}


	public static SqliteCommand CreateSelectCommand(this SqliteConnection connection, object obj,
	                                                params string[] columns)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.Select, null, columns);
	}

	public static SqliteCommand CreateSelectCommand<T>(this SqliteConnection connection,
	                                                   Dictionary<string, object?> columnNameValuePairConditions,
	                                                   params string[] columns)
	{
		return CreateSqlCommand<T>(connection, SqlOperation.Select, columnNameValuePairConditions, columns);
	}

	/// <summary> creates a query that searches for rows that match the given object's <see cref="TableAttribute" />.
	/// Rows that match either the object's properties OR dictionary values will be returned.</summary> 
	///
	/// <param name="connection"> The connection to the database to use</param>
	/// <param name="obj"> The object to search for like rows in its associated table</param>
	/// <param name="columnNameValuePairConditions">specifies the columns and values to search against in the where clause.
	/// Each key is a column name and each value is an object that represents what you want to search for in that column.</param>
	/// <param name="columns">
	/// /// the columns to be returned in the select statement; if null or empty, all columns will be returned
	/// </param>
	///
	/// <returns> A sql command that can be used to search for rows that match the given object or dictionary values.</returns>
	/// <remarks>because of the flexibility of sqlite each values will be wrapped in percentage signs (%) searched with the LIKE operator</remarks>
	public static SqliteCommand CreateSelectLikeCommand(this SqliteConnection connection, object obj,
	                                                    Dictionary<string, object?> columnNameValuePairConditions,
	                                                    params string[] columns)
	{
		return CreateSqlCommand(connection, obj, SqlOperation.SelectLike, columnNameValuePairConditions, columns);
	}

	private static SqliteCommand CreateSqlCommand<T>(SqliteConnection connection, SqlOperation sqlOperation,
	                                                 Dictionary<string, object?>? idColumnNameValuePairs = null,
	                                                 params string[] selectColumns)
	{
		var tableName = typeof(T).GetTableName();
		if(tableName == null)
			throw new ArgumentException("object does not have a table in the database");

		var columnNames = typeof(T).GetColumnNames();
		var cmdTxt = CreateCommandText(columnNames, sqlOperation, selectColumns)
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

	private static SqliteCommand CreateSqlCommand(SqliteConnection? connection, object obj, SqlOperation sqlOperation,
	                                              Dictionary<string, object?>? columnNameValuePairConditions = null,
	                                              params string[] selectColumns)
	{
		var tableName = obj.GetTableName();
		if(tableName == null)
			throw new ArgumentException("object does not have a table in the database", nameof(obj));

		var columnNameValuePairs = obj.GetPropertyColumnNameValuePairs();
		var columnNames = columnNameValuePairs.Keys.ToList();

		//todo make separate method to additional where clauses or createcommandtext method
		// ? add id column to columnNames list so it's included in the statements where clause
		if(columnNameValuePairConditions != null)
			foreach(var (columnName, _) in columnNameValuePairConditions)
				columnNames.Add(columnName);

		var returnColumns = selectColumns.Length == 0 ? string.Join(", ", selectColumns) : "*";
		var conditionsList = obj.GetColumnNames();
		if(columnNameValuePairConditions != null)
			conditionsList.AddRange(columnNameValuePairConditions.Keys);
		conditionsList = conditionsList.Select(columnName => $":{columnName} LIKE %@{columnName}%").ToList();
		var conditions = string.Join(" OR ", conditionsList);
		var cmdTxt = $"SELECT {returnColumns} FROM {tableName} WHERE {conditions};";
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

//todo: merge both condition dictionaries and paramterize in one loop
		if(columnNameValuePairConditions != null)
			foreach(var (columnName, value) in columnNameValuePairConditions)
			{
				// cmd.Parameters.AddWithValue($":{columnName}", columnName);
				cmd.CommandText =
					cmd.CommandText.Replace($":{columnName}", columnName); // could be optimized with a replace all
				cmd.Parameters.AddWithNullableValue($"@{value}", value);
			}

		cmd.Prepare();
		return cmd;
	}

	// todo: edit to allow for given dict of conditions (i.e. { "name": "bob", "age": 30 }) sort of thing
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
				var returnColumns = selectColumns.Length == 0 ? string.Join(", ", selectColumns) : "*";
				return $"SELECT {returnColumns} FROM $table WHERE {condition};";
			case SqlOperation.SelectLike:
				throw new NotImplementedException();
			case SqlOperation.SelectAll:
				returnColumns = selectColumns.Length == 0 ? string.Join(", ", selectColumns) : "*";
				return $"SELECT {returnColumns} FROM $table;";
			case SqlOperation.Drop:
				return "DROP TABLE $table;";
			case SqlOperation.DropIfExists:
				return "DROP TABLE IF EXISTS $table;";
			default:
				throw new ArgumentOutOfRangeException(nameof(sqlOperation), sqlOperation, null);
		}
	}

	#region Nested type: SqlOperation

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
		DropIfExists,
		SelectLike
	}

	#endregion

	#region extensions

	/// <summary> The GetPropertyValue function returns the value of a property on an object.</summary>
	/// <param name="obj"> The object to get the property value from.</param>
	/// <param name="propertyName"> The name of the property to get.</param>
	/// <returns>
	///     The value of the property specified by the <see cref="propertyName" /> parameter. if no such property exists,
	///     it returns null.
	/// </returns>
	private static object? GetPropertyValue(this object obj, string propertyName)
	{
		return obj.GetType()
		          .GetProperty(propertyName)
		          ?.GetValue(obj);
	}

	#region class sql attributes

	/// <summary>
	///     return the related table name of the object's class
	/// </summary>
	/// <param name="obj">the object to get the table name</param>
	/// <returns>return <see cref="TableAttribute" />'s name from object of the object</returns>
	private static string? GetTableName<T>(this T obj)
	{
		return typeof(T).GetTableName();
	}

	private static string? GetTableName(this Type type)
	{
		return type.GetCustomAttribute<TableAttribute>()?.Name;
	}

	/// <summary>
	///     return the related column name of the given property
	/// </summary>
	/// <param name="obj">the object to get the column name</param>
	/// <param name="propertyName">the object's property name to get the column name</param>
	/// <returns>return <see cref="ColumnAttribute" />'s name from object of the object</returns>
	private static string? GetColumnName(this object obj, string propertyName)
	{
		return obj.GetType()
		          .GetProperty(propertyName)
		          ?.GetCustomAttribute<ColumnAttribute>()
		          ?.Name;
	}

	/// <summary>
	///     return the all non-null column names of the given object's properties
	/// </summary>
	/// <param name="obj">the object to get the column names</param>
	/// <returns>return a list of <see cref="ColumnAttribute" />'s <see cref="ColumnAttribute.Name" /></returns>
	private static List<string> GetColumnNames(this object obj)
	{
		return obj.GetType().GetColumnNames();
	}

	private static List<string> GetColumnNames(this Type type)
	{
		return type
		       .GetProperties()
		       .Select(property => property.GetCustomAttribute<ColumnAttribute>()?.Name)
		       .Where(name => name != null)
		       .ToList()!;
	}

	/// <summary>
	///     return a dictionary of the given object's properties and their values.
	///     <para>
	///         where the key is the property name and the value is the property value
	///     </para>
	/// </summary>
	/// <param name="obj">the object to construct its property name and value dictionary</param>
	/// <returns>return a dictionary of the given object's properties and their values</returns>
	private static Dictionary<string, object?> GetPropertyNameValuePairs(this object obj)
	{
		return obj.GetType()
		          .GetProperties()
		          .ToDictionary(property => property.Name, property => property.GetValue(obj));
	}

	/// <summary>
	///     creates a dictionary of the given object's properties <see cref="ColumnAttribute" />'s
	///     <see cref="ColumnAttribute.Name" />
	///     and values
	/// </summary>
	/// <param name="obj"> The object to get the property's column name and value pairs from.</param>
	/// <returns> A dictionary of column names and values.</returns>
	private static Dictionary<string, object?> GetPropertyColumnNameValuePairs(this object obj)
	{
		return obj.GetPropertyNameValuePairs()
		          .Where(pair => obj.GetColumnName(pair.Key) != null)
		          .ToDictionary(pair => obj.GetColumnName(pair.Key)!, pair => pair.Value);
	}

	#endregion

	#endregion
}