namespace resume_builder.models.database;

public partial class Database
{
	public void Wipe()
	{
		var cmd = MainConnection.CreateCommand();
		foreach(var table in TemplateTableStructure.Keys)
			try
			{
				cmd.CommandText = $"DELETE FROM \"{table}\";";
				cmd.ExecuteNonQuery();
			}
			catch(Exception)
			{
				if(IsInitialized())
					throw;
			}
	}
}