namespace resume_builder.models;

/// <summary>
///     defines the related sql column name in project's database
/// escape's and wraps the name in quotes
/// </summary>
public class SqlColumnNameAttribute : Attribute
{
	//todo: ~~escape and~~ validate sql names
	private readonly string _name = null!;

	public string Name
	{
		get => $"\"{_name}\"";
		init => _name = value;
	}

	public SqlColumnNameAttribute(string name)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name), "Name (of/for related sql column) cannot be null");
	}
}