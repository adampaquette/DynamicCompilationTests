﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Generator;

[Generator]
public partial class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (x, _) => x is ClassDeclarationSyntax c,
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(x => x is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            return;
        }        

        var source = """
            using System;

            namespace Test
            {
                public class MyClass
                {
                }
            }
            """;

        context.AddSource($"MyClass.g.cs", SourceText.From(source, Encoding.UTF8));
        
        var dynamicCode = Executor.Execute(compilation.SyntaxTrees.First().ToString());
        context.AddSource($"DynamicClass.g.cs", SourceText.From(dynamicCode, Encoding.UTF8));
    }
}
