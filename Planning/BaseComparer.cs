namespace etvctl.Planning;

public class BaseComparer
{
    protected static bool HasStringChanges(string? one, string? two)
    {
        if (string.IsNullOrWhiteSpace(one) && string.IsNullOrWhiteSpace(two))
        {
            return false;
        }

        return one != two;
    }
}
