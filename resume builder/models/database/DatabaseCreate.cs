namespace resume_builder.models.database;

public partial class Database
{
	private void CreateTemporaryTermsTable(IReadOnlyList<string> terms)
	{
		using var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "CREATE TEMPORARY TABLE term(query TEXT);";
		cmd.ExecuteNonQuery();

		cmd.CommandText = $"INSERT INTO term VALUES ";
		var values = string.Join(",", terms.Select((_, i) => $"($term{i})"));
		cmd.CommandText += values;
		for(var i = 0; i < terms.Count; i++)
			cmd.Parameters.AddWithValue($"$term{i}", terms[i].Surround("%"));
		cmd.Prepare();
		cmd.ExecuteNonQuery();
	}
}