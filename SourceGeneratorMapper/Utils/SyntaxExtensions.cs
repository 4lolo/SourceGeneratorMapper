using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGeneratorMapper.Utils;

public static class SyntaxExtensions
{
    public static string? GetNamespaceName(this SyntaxNode? syntax)
    {
        while (syntax is not null)
        {
            if (syntax is NamespaceDeclarationSyntax ns)
            {
                return ns.Name.ToFullString();
            }

            if (syntax is FileScopedNamespaceDeclarationSyntax fileNs)
            {
                return fileNs.Name.ToFullString();
            }

            syntax = syntax.Parent;
        }

        return null;
    }
}