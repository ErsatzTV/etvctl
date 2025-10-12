using etvctl.Api;

namespace etvctl.Models;

public class PlanModel
{
    public required ChangeSet<SmartCollectionModel, SmartCollectionResponseModel> SmartCollections { get; set; }

    public bool IsEmpty()
    {
        return ToAdd() == 0 && ToUpdate() == 0 && ToRemove() == 0;
    }

    public int ToAdd()
    {
        return SmartCollections.ToAdd.Count;
    }

    public int ToUpdate()
    {
        return SmartCollections.ToUpdate.Count;
    }

    public int ToRemove()
    {
        return SmartCollections.ToRemove.Count;
    }
}
