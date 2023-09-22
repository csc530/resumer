namespace resume_builder
{
	internal class SqlTableNameAttribute : Attribute
	{
		//todo: ~~escape and~~ validate sql names
		public string Name { get; init; }

		public string EscapedName => $"\"{Name}\"";

		public SqlTableNameAttribute(string name)
		{
			Name = name ??
			       throw new ArgumentNullException(nameof(name), "Name (of/for related sql table) cannot be null");
		}
	}
}