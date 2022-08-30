using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SourceGeneratorMapper.Utils;

public static class ContextExtensions
{
    public static void AddSourceFromManifestResource(this IncrementalGeneratorPostInitializationContext context, string fileName, string resourceManifestName)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceManifestName);

        if (stream == null)
        {
            throw new ArgumentException($"Could not find {resourceManifestName} manifest");
        }
        
        SourceText source = SourceText.From(stream, Encoding.UTF8, canBeEmbedded: true);
        context.AddSource(fileName, source);        
    }
    
    // Logging
    private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("SC0001", "Warning", "{0}", "Category", DiagnosticSeverity.Warning, true);
    
    public static void Warn(this SourceProductionContext context, string message)
    {
        context.ReportDiagnostic(Diagnostic.Create(Descriptor, null, message));
    }
}