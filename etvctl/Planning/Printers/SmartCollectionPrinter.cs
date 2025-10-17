using System.Diagnostics.CodeAnalysis;
using etvctl.Api;
using etvctl.Generator;
using etvctl.Models;

namespace etvctl.Planning;

[EtvPrinter("Smart Collection", typeof(SmartCollectionModel), typeof(SmartCollectionResponseModel))]
[SuppressMessage("ReSharper", "UnusedParameter.Local")]
public partial class SmartCollectionPrinter : BaseComparer
{
    private static bool ShouldIncludeProperty(SmartCollectionModel smartCollection, string snakeCasePropertyName) =>
        true;

    private static bool ShouldIncludeProperty(
        SmartCollectionResponseModel smartCollection,
        string snakeCasePropertyName) =>
        true;
}
