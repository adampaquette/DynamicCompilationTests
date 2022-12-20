﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using DynamicCompilationNetStandard2._0;

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

        try
        {
            var source = """
                using System.Linq.Expressions;
                using System;
                using Core;

                namespace Consumer
                {
                    public class MyTask : ITask
                    {
                        public void CanRun<T>(Expression<Func<T, bool>> predicate)
                        {
                        }

                        public void Run()
                        {
                            Console.WriteLine("Finished");
                        }
                    }
                }
                """;

            Executor.Execute(source);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}