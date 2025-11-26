namespace Domain.ValueObjects;

public class Role
{
    public string DisplayName {get; set;} = string.Empty;
    public bool IsAdmin {get; set;} = false;
}