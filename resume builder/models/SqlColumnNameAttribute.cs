namespace resume_builder.models;

/// <summary>
///     defines the related sql column name in project's database
/// escape's and wraps the name in quotes
/// </summary>
public class SqlColumnNameAttribute : Attribute
{
	//todo: ~~escape and~~ validate sql names
	public string Name { get; init; }

	public string EscapedName => $"\"{Name}\"";

	public SqlColumnNameAttribute(string name)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name), "Name (of/for related sql column) cannot be null");
	}
}