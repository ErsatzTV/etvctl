using System;

namespace etvctl.Generator;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class EtvPrinterOrderAttribute(int order) : Attribute
{
    public int Order { get; } = order;
}
