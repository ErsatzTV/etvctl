using etvctl.Api;
using etvctl.Models;

namespace etvctl.Planning;

public class SmartCollectionPlanner(IErsatzTVv1 client)
    : BasePlanner<SmartCollectionModel, SmartCollectionResponseModel>
{
    public override async Task<ChangeSet<SmartCollectionModel, SmartCollectionResponseModel>> Plan(
        TemplateModel templateModel,
        CancellationToken cancellationToken)
    {
        // load current smart collections
        ICollection<SmartCollectionResponseModel> currentSmartCollections =
            await client.GetSmartCollections(cancellationToken);

        // diff and plan
        var toAdd = templateModel.SmartCollections
            .Where(sc => currentSmartCollections.All(csc => csc.Name != sc.Name))
            .Where(sc => sc.Rename?.From is null)
            .ToList();

        var toUpdate = templateModel.SmartCollections
            .Where(sc => currentSmartCollections.Any(csc => csc.Name == sc.Name && HasStringChanges(csc.Query, sc.Query)))
            .Select(sc => new Tuple<SmartCollectionModel, SmartCollectionResponseModel>(
                sc,
                currentSmartCollections.First(csc => csc.Name == sc.Name)))
            .ToList();

        foreach (var sc in templateModel.SmartCollections.Where(sc => !string.IsNullOrWhiteSpace(sc.Rename?.From)))
        {
            var from = currentSmartCollections.FirstOrDefault(csc => string.Equals(csc.Name, sc.Rename?.From));
            if (from != null)
            {
                toUpdate.Add(new Tuple<SmartCollectionModel, SmartCollectionResponseModel>(sc, from));
            }
        }

        var toRemove = currentSmartCollections
            .Where(sc => templateModel.SmartCollections.All(csc => csc.Name != sc.Name))
            .Where(sc => templateModel.SmartCollections.All(csc => csc.Rename?.From != sc.Name))
            .ToList();

        return new ChangeSet<SmartCollectionModel, SmartCollectionResponseModel>
        {
            ToAdd = toAdd,
            ToRemove = toRemove,
            ToUpdate = toUpdate
        };
    }

    public override async Task Apply(PlanModel plan, CancellationToken cancellationToken)
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
    }
}
