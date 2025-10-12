namespace etvctl.Models;

public record SmartCollectionModel
{
    public string? Name { get; set; }
    public string? Query { get; set; }
    public RenameModel? Rename { get; set; }
}
