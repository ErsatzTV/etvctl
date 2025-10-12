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
            .Where(sc => sc.Rename?.From is null)
            .ToList();

        var toUpdate = rootModel.SmartCollections
            .Where(sc => currentSmartCollections.Any(csc => csc.Name == sc.Name && csc.Query != sc.Query))
            .Select(sc => new Tuple<SmartCollectionModel, SmartCollectionResponseModel>(sc, currentSmartCollections.First(csc => csc.Name == sc.Name)))
            .ToList();

        foreach (var sc in rootModel.SmartCollections.Where(sc => !string.IsNullOrWhiteSpace(sc.Rename?.From)))
        {
            var from = currentSmartCollections.FirstOrDefault(csc => string.Equals(csc.Name, sc.Rename?.From));
            if (from != null)
            {
                toUpdate.Add(new Tuple<SmartCollectionModel, SmartCollectionResponseModel>(sc, from));
            }
        }

        var toRemove = currentSmartCollections
            .Where(sc => rootModel.SmartCollections.All(csc => csc.Name != sc.Name))
            .Where(sc => rootModel.SmartCollections.All(csc => csc.Rename?.From != sc.Name))
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

    public async Task<bool> ApplyPlan(
        IErsatzTVv1 client,
        PlanModel plan,
        CancellationToken cancellationToken = default)
    {
        foreach (var toAdd in plan.SmartCollections.ToAdd)
        {
            if (string.IsNullOrWhiteSpace(toAdd.Name) || string.IsNullOrWhiteSpace(toAdd.Query))
            {
                continue;
            }

            await client.CreateSmartCollection(
                new CreateSmartCollection { Name = toAdd.Name, Query = toAdd.Query },
                cancellationToken);
        }

        foreach ((SmartCollectionModel toUpdateNew, SmartCollectionResponseModel toUpdateOld) in plan.SmartCollections
                     .ToUpdate)
        {
            if (string.IsNullOrWhiteSpace(toUpdateNew.Query) || string.IsNullOrWhiteSpace(toUpdateNew.Name))
            {
                continue;
            }

            await client.UpdateSmartCollection(
                new UpdateSmartCollection { Id = toUpdateOld.Id, Name = toUpdateNew.Name, Query = toUpdateNew.Query },
                cancellationToken);
        }

        foreach (var toRemove in plan.SmartCollections.ToRemove)
        {
            await client.DeleteSmartCollection(toRemove.Id, cancellationToken);
        }

        return true;
    }
}
