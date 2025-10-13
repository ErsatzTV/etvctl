using etvctl.Api;

namespace etvctl.Models;

public record SmartCollectionModel
{
    public SmartCollectionModel()
    {
    }

    public SmartCollectionModel(SmartCollectionResponseModel model)
    {
        Name = model.Name;
        Query = model.Query;
    }

    public string? Name { get; set; }
    public string? Query { get; set; }
    public RenameModel? Rename { get; set; }
}
