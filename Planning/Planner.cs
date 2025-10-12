using etvctl.Api;
using etvctl.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace etvctl.Planning;

public class Planner
{
    public async Task<PlanModel> DevelopPlan(
        ConfigModel config,
        IErsatzTVv1 client,
        CancellationToken cancellationToken = default)
    {
        var deserializer = new StaticDeserializerBuilder(new YamlStaticContext())
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        // load template smart collections
        string templateText = await File.ReadAllTextAsync(config.Template!, cancellationToken);
        var rootModel = deserializer.Deserialize<RootModel>(templateText);

        // load current smart collections
        ICollection<SmartCollectionResponseModel> currentSmartCollections =
            await client.GetSmartCollections(cancellationToken);

        // diff and plan
        var toAdd = rootModel.SmartCollections
            .Where(sc => currentSmartCollections.All(csc => csc.Name != sc.Name))
            .ToList();

        var toUpdate = rootModel.SmartCollections
            .Where(sc => currentSmartCollections.Any(csc => csc.Name == sc.Name && csc.Query != sc.Query))
            .ToList();

        var toRemove = currentSmartCollections
            .Where(sc => rootModel.SmartCollections.All(csc => csc.Name != sc.Name))
            .ToList();

        return new PlanModel
        {
            SmartCollections = new ChangeSet<SmartCollectionModel, SmartCollectionResponseModel>
            {
                ToAdd = toAdd,
                ToRemove = toRemove,
                ToUpdate = toUpdate
            }
        };
    }
}
