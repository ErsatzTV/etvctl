using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text.Json;

namespace etvctl.Generator;

[Generator]
public class PrinterGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = "etvctl.Generator.EtvPrinterAttribute";
    private const string OrderAttributeFullName = "etvctl.Generator.EtvPrinterOrderAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
            transform: (ctx, _) => (ClassDeclarationSyntax)ctx.Node
        );

        IncrementalValuesProvider<(ClassDeclarationSyntax Left, Compilation Right)> classDeclarationsWithCompilation =
            classDeclarations.Combine(context.CompilationProvider);

        IncrementalValuesProvider<GenerationTarget?> generationTargets = classDeclarationsWithCompilation.Select((tuple, cancellationToken) =>
        {
            (ClassDeclarationSyntax? classDeclaration, Compilation? compilation) = tuple;
            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);

            var attribute = classSymbol?.GetAttributes()
                .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == AttributeFullName);

            if (attribute == null || attribute.ConstructorArguments.Length != 3)
            {
                return null;
            }

            var title = attribute.ConstructorArguments[0].Value as string;
            var modelType = attribute.ConstructorArguments[1].Value as INamedTypeSymbol;
            var apiType = attribute.ConstructorArguments[2].Value as INamedTypeSymbol;

            if (title == null || modelType == null || apiType == null)
            {
                return null;
            }

            return new GenerationTarget(classSymbol!, title, modelType, apiType);
        });

        IncrementalValuesProvider<GenerationTarget?> validTargets = generationTargets.Where(t => t is not null);

        context.RegisterSourceOutput(validTargets, (spc, target) => Execute(target!, spc));
    }

    private static void Execute(GenerationTarget target, SourceProductionContext context)
    {
        string className = target.TargetClass.Name;
        string classNamespace = target.TargetClass.ContainingNamespace.ToDisplayString();

        string title = target.Title;
        string resourceName = target.Title.Replace(" ", "_").ToLowerInvariant();
        string modelTypeName = target.ModelTypeSymbol.ToDisplayString();
        string apiTypeName = target.ApiTypeSymbol.ToDisplayString();

        var modelProperties = target.ModelTypeSymbol.GetMembers().OfType<IPropertySymbol>()
            .Where(p => p.Name != "EqualityContract")
            .ToList();
        var sortedProperties = modelProperties.Select(p =>
            {
                var orderAttribute = p.GetAttributes()
                    .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == OrderAttributeFullName);
                int order = int.MaxValue;
                if (orderAttribute != null && orderAttribute.ConstructorArguments.Length == 1 &&
                    orderAttribute.ConstructorArguments[0].Value is int orderValue)
                {
                    order = orderValue;
                }

                return (Property: p, Order: order);
            })
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Property.Name)
            .Select(x => x.Property)
            .ToList();

        int longestPropertyLength = modelProperties
            .Select(p => JsonNamingPolicy.SnakeCaseLower.ConvertName(p.Name).Length)
            .Max();

        string nameSpaces = " ".PadLeft(longestPropertyLength - 4 + 1);

        var sourceBuilder = new StringBuilder();
        sourceBuilder.AppendLine("using etvctl.Api;");
        sourceBuilder.AppendLine("using etvctl.Models;");
        sourceBuilder.AppendLine("using Spectre.Console;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine($"namespace {classNamespace};");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine($"public partial class {className}");
        sourceBuilder.AppendLine("{");
        sourceBuilder.AppendLine($"    public static void Print(ChangeSet<{modelTypeName}, {apiTypeName}> changeSet)");
        sourceBuilder.AppendLine("    {");
        sourceBuilder.AppendLine("        if (changeSet.ToAdd.Count != 0)");
        sourceBuilder.AppendLine("        {");
        sourceBuilder.AppendLine("            foreach (var add in changeSet.ToAdd)");
        sourceBuilder.AppendLine("            {");
        sourceBuilder.AppendLine($"                AnsiConsole.MarkupLine($\"  # {title} \\\"{{add.Name}}\\\" will be created\");");
        sourceBuilder.AppendLine($"                AnsiConsole.MarkupInterpolated($\"  [green]+ resource \\\"{resourceName}\\\" \\\"{{add.Name}}\\\" {{{{[/]\\n\");");
        sourceBuilder.AppendLine($"                AnsiConsole.MarkupInterpolated($\"  [green]    + name:{nameSpaces}\\\"{{add.Name}}\\\"[/]\\n\");");
        foreach (var property in sortedProperties)
        {
            string snakeCaseName = JsonNamingPolicy.SnakeCaseLower.ConvertName(property.Name);
            if (snakeCaseName is "rename" or "name")
            {
                continue;
            }

            string spaces = " ".PadLeft(Math.Max(longestPropertyLength - snakeCaseName.Length + 1, 1));

            sourceBuilder.AppendLine($"                if (ShouldIncludeProperty(add, \"{snakeCaseName}\"))");
            if (property.Type.SpecialType == SpecialType.System_String)
            {
                sourceBuilder.AppendLine($"                    AnsiConsole.MarkupInterpolated($\"  [green]    + {snakeCaseName}:{spaces}\\\"{{add.{property.Name}}}\\\"[/]\\n\");");
            }
            else
            {
                sourceBuilder.AppendLine($"                    AnsiConsole.MarkupLine($\"  [green]    + {snakeCaseName}:{spaces}\\\"{{add.{property.Name}}}\\\"[/]\");");
            }
        }

        sourceBuilder.AppendLine("                AnsiConsole.MarkupLine(\"  [green]  }[/]\");");
        sourceBuilder.AppendLine("            }");
        sourceBuilder.AppendLine("            AnsiConsole.MarkupLine(\"\");");
        sourceBuilder.AppendLine("        }");

        sourceBuilder.AppendLine("        if (changeSet.ToUpdate.Count != 0)");
        sourceBuilder.AppendLine("        {");
        sourceBuilder.AppendLine("            foreach (var (newValue, oldValue) in changeSet.ToUpdate)");
        sourceBuilder.AppendLine("            {");
        sourceBuilder.AppendLine($"                AnsiConsole.MarkupLine($\"  # {title} \\\"{{oldValue.Name}}\\\" will be changed\");");
        sourceBuilder.AppendLine($"                AnsiConsole.MarkupInterpolated($\"  [yellow]~ resource \\\"{resourceName}\\\" \\\"{{oldValue.Name}}\\\" {{{{[/]\\n\");");
        foreach (var property in sortedProperties)
        {
            string snakeCaseName = JsonNamingPolicy.SnakeCaseLower.ConvertName(property.Name);
            if (snakeCaseName is "rename")
            {
                continue;
            }

            string spaces = " ".PadLeft(Math.Max(longestPropertyLength - snakeCaseName.Length + 1, 1));

            sourceBuilder.AppendLine($"                if (ShouldIncludeProperty(oldValue, \"{snakeCaseName}\") || ShouldIncludeProperty(newValue, \"{snakeCaseName}\"))");
            if (property.Type.SpecialType == SpecialType.System_String)
            {
                sourceBuilder.AppendLine($"                    if (HasStringChanges(oldValue.{property.Name}, newValue.{property.Name}))");
                sourceBuilder.AppendLine("                    {");
                sourceBuilder.AppendLine($"                        AnsiConsole.MarkupInterpolated($\"  [red]    - {snakeCaseName}:{spaces}\\\"{{oldValue.{property.Name}}}\\\"[/]\\n\");");
                sourceBuilder.AppendLine($"                        AnsiConsole.MarkupInterpolated($\"  [green]    + {snakeCaseName}:{spaces}\\\"{{newValue.{property.Name}}}\\\"[/]\\n\");");
                sourceBuilder.AppendLine("                    }");
            }
            else
            {
                sourceBuilder.AppendLine($"                    if (oldValue.{property.Name} != newValue.{property.Name})");
                sourceBuilder.AppendLine("                    {");
                sourceBuilder.AppendLine($"                        AnsiConsole.MarkupLine($\"  [red]    - {snakeCaseName}:{spaces}\\\"{{oldValue.{property.Name}}}\\\"[/]\");");
                sourceBuilder.AppendLine($"                        AnsiConsole.MarkupLine($\"  [green]    + {snakeCaseName}:{spaces}\\\"{{newValue.{property.Name}}}\\\"[/]\");");
                sourceBuilder.AppendLine("                    }");
            }
        }

        sourceBuilder.AppendLine("                AnsiConsole.MarkupLine(\"  [yellow]  }[/]\");");
        sourceBuilder.AppendLine("            }");
        sourceBuilder.AppendLine("            AnsiConsole.MarkupLine(\"\");");
        sourceBuilder.AppendLine("        }");

        sourceBuilder.AppendLine("        if (changeSet.ToRemove.Count != 0)");
        sourceBuilder.AppendLine("        {");
        sourceBuilder.AppendLine("            foreach (var remove in changeSet.ToRemove)");
        sourceBuilder.AppendLine("            {");
        sourceBuilder.AppendLine($"                AnsiConsole.MarkupLine($\"  # {title} \\\"{{remove.Name}}\\\" will be deleted\");");
        sourceBuilder.AppendLine($"                AnsiConsole.MarkupInterpolated($\"  [red]- resource \\\"{resourceName}\\\" \\\"{{remove.Name}}\\\" {{{{[/]\\n\");");
        foreach (var property in sortedProperties)
        {
            string snakeCaseName = JsonNamingPolicy.SnakeCaseLower.ConvertName(property.Name);
            if (snakeCaseName is "rename")
            {
                continue;
            }

            string spaces = " ".PadLeft(Math.Max(longestPropertyLength - snakeCaseName.Length + 1, 1));

            sourceBuilder.AppendLine($"                if (ShouldIncludeProperty(remove, \"{snakeCaseName}\"))");
            if (property.Type.SpecialType == SpecialType.System_String)
            {
                sourceBuilder.AppendLine($"                    AnsiConsole.MarkupInterpolated($\"  [red]    - {snakeCaseName}:{spaces}\\\"{{remove.{property.Name}}}\\\"[/]\\n\");");
            }
            else
            {
                sourceBuilder.AppendLine($"                    AnsiConsole.MarkupLine($\"  [red]    - {snakeCaseName}:{spaces}\\\"{{remove.{property.Name}}}\\\"[/]\");");
            }
        }

        sourceBuilder.AppendLine("                AnsiConsole.MarkupLine(\"  [red]  }[/]\");");
        sourceBuilder.AppendLine("            }");
        sourceBuilder.AppendLine("            AnsiConsole.MarkupLine(\"\");");
        sourceBuilder.AppendLine("        }");

        sourceBuilder.AppendLine("    }");
        sourceBuilder.AppendLine("}");

        context.AddSource($"{className}.g.cs", sourceBuilder.ToString());
    }

    private sealed record GenerationTarget(
        INamedTypeSymbol TargetClass,
        string Title,
        INamedTypeSymbol ModelTypeSymbol,
        INamedTypeSymbol ApiTypeSymbol)
    {
        public INamedTypeSymbol TargetClass { get; } = TargetClass;
        public string Title { get; } = Title;
        public INamedTypeSymbol ModelTypeSymbol { get; } = ModelTypeSymbol;
        public INamedTypeSymbol ApiTypeSymbol { get; } = ApiTypeSymbol;
    }
}
