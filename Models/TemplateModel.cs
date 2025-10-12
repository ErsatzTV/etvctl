namespace etvctl.Models;

public class TemplateModel
{
    //public List<ChannelModel> Channels { get; set; } = [];
    public List<SmartCollectionModel> SmartCollections { get; set; } = [];

    public bool IsEmpty() => SmartCollections.Count == 0;
}
