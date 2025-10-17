using etvctl.Api;
using etvctl.Generator;

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

    [EtvPrinterOrder(1)]
    public string? Name { get; set; }

    [EtvPrinterOrder(2)]
    public string? Query { get; set; }

    public RenameModel? Rename { get; set; }
}
