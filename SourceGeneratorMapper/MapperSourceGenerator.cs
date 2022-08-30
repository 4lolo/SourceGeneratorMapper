using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGeneratorMapper.Utils;

namespace SourceGeneratorMapper
{
    [Generator]
    public class MapperSourceGenerator : IIncrementalGenerator
    {
        // https://medium.com/@saravananganesan/exploring-c-source-generators-part-1-understanding-registerpostinitializationoutput-49dfd08ca052
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            #if DEBUG
            if (!Debugger.IsAttached)
            {
                // Debugger.Launch();
            }
            #endif
            
            initContext.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSourceFromManifestResource("MapperAttribute.g.cs", "SourceGeneratorMapper.Templates.MapperAttribute.cs");
                ctx.AddSourceFromManifestResource("MappingAttribute.g.cs", "SourceGeneratorMapper.Templates.MappingAttribute.cs");
            });
            
            // Do a simple filter for enums
            IncrementalValuesProvider<ClassDeclarationSyntax> mapperDeclarations = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsSyntaxTargetForGeneration, GetSemanticTargetForGeneration)
                .Where(syntax => syntax is not null)!;

            // Combine the selected enums with the `Compilation`
            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndEnums
                = initContext.CompilationProvider.Combine(mapperDeclarations.Collect());
            
            // Generate the source using the compilation and enums
            initContext.RegisterSourceOutput(compilationAndEnums, static (spc, source) => Execute(spc, source.Item1, source.Item2));
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
        }

        static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context, CancellationToken token)
        {
            // we know the node is a EnumDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            // loop through all the attributes on the method
            foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    // Is the attribute the [MapperAttribute] attribute?
                    if (fullName == "SourceGeneratorMapper.Attributes.MapperAttribute")
                    {
                        // return the enum
                        return classDeclarationSyntax;
                    }
                }
            }

            // we didn't find the attribute we were looking for
            return null;
        }
        
        static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes)
        {
            foreach (ClassDeclarationSyntax classDeclarationSyntax in classes.Distinct())
            {
                ExecuteOne(context, compilation, classDeclarationSyntax);
            }
        }

        private static void ExecuteOne(SourceProductionContext context, Compilation compilation, ClassDeclarationSyntax classDeclarationSyntax)
        {
            string? namespaceName = classDeclarationSyntax.GetNamespaceName()?.AppendString(".") ?? string.Empty;
            string hintName = $"{namespaceName}{classDeclarationSyntax.Identifier.Text}.g.cs";
            context.Warn(hintName);
            
            // context.AddSource();
        }
    }
}