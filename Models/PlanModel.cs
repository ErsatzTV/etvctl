using etvctl.Api;

namespace etvctl.Models;

public class PlanModel
{
    public required ChangeSet<FFmpegProfileModel, FFmpegFullProfileResponseModel> FFmpegProfiles { get; set; }
    public required ChangeSet<SmartCollectionModel, SmartCollectionResponseModel> SmartCollections { get; set; }

    public bool IsEmpty()
    {
        return ToAdd() == 0 && ToUpdate() == 0 && ToRemove() == 0;
    }

    public int ToAdd()
    {
        return FFmpegProfiles.ToAdd.Count + SmartCollections.ToAdd.Count;
    }

    public int ToUpdate()
    {
        return FFmpegProfiles.ToUpdate.Count + SmartCollections.ToUpdate.Count;
    }

    public int ToRemove()
    {
        return FFmpegProfiles.ToRemove.Count + SmartCollections.ToRemove.Count;
    }
}
