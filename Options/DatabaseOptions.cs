namespace SAQS_kolla_backend.Options;

public class DatabaseOptions()
{
    public required string SqliteConnectionString {get; set;}

    public bool Validate()
    {
        return string.IsNullOrEmpty(SqliteConnectionString) == false;
    }
}