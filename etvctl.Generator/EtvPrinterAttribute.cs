using System;

namespace etvctl.Generator;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EtvPrinterAttribute(string title, Type modelType, Type apiType) : Attribute
{
    public string Title { get; } = title;
    public Type ModelType { get; } = modelType;
    public Type ApiType { get; } = apiType;
}
