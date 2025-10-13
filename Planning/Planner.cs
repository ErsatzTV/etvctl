using etvctl.Api;
using etvctl.Models;
using etvctl.Models.Config;

namespace etvctl.Planning;

public class Planner
{
    public async Task<PlanModel> DevelopPlan(
        ConfigModel config,
        IErsatzTVv1 client,
        CancellationToken cancellationToken = default)
    {
        var templateModel = await YamlReader.ReadTemplate(config, cancellationToken);

        var smartCollectionPlanner = new SmartCollectionPlanner(client);

        return new PlanModel
        {
            SmartCollections = await smartCollectionPlanner.Plan(templateModel, cancellationToken)
        };
    }

    public async Task<bool> ApplyPlan(IErsatzTVv1 client, PlanModel plan, CancellationToken cancellationToken = default)
    {
        var smartCollectionPlanner = new SmartCollectionPlanner(client);
        await smartCollectionPlanner.Apply(plan, cancellationToken);

        return true;
    }
}
