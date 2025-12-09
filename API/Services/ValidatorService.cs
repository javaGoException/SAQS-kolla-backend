namespace SAQS_kolla_backend.API;

public class ValidatorService
{
    public bool isGuidValid(string? guid)
    {
        if (string.IsNullOrEmpty(guid))
        {
            return false;    
        }

        Guid parsedGuid;
        try
        {
            parsedGuid = Guid.Parse(guid);
        } 
        catch (FormatException)
        {
            return false;
        }

        return true;
    }
}