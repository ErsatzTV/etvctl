namespace etvctl.Models;

public class ChangeSet<T1, T2>
{
    public List<T1> ToAdd { get; set; } = [];
    public List<T1> ToUpdate { get; set; } = [];
    public List<T2> ToRemove { get; set; } = [];
}
