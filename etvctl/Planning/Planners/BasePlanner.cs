using etvctl.Models;

namespace etvctl.Planning;

public abstract class BasePlanner<T1, T2> : BaseComparer
{
    public abstract Task<ChangeSet<T1, T2>> Plan(TemplateModel templateModel, CancellationToken cancellationToken);

    public abstract Task Apply(PlanModel plan, CancellationToken cancellationToken);
}
