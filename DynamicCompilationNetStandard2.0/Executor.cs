using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DynamicCompilationNetStandard2._0
{
    public static class Executor
    {
        public static void Execute(string source)
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(source);
            var compilation = CSharpCompilation.Create(assemblyName: Path.GetRandomFileName())
                .WithReferenceAssemblies(ReferenceAssemblyKind.NetStandard20)
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(ITask).Assembly.Location))
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(syntaxTree);

            ////IMPLEMENTATION ASSEMBLIES
            //var implementationAssemblies = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            //var references = implementationAssemblies
            //    .Select(p => MetadataReference.CreateFromFile(p))
            //    .ToList();
            //compilation = compilation.AddReferences(references);

            ////REFERENCE ASSEMBLIES
            //var netstandardFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget\\packages\\netstandard.library\\2.0.3\\build\\netstandard2.0\\ref");
            //var referenceAssemblies = Directory.GetFiles(netstandardFolder)
            //    .Where(x => x.EndsWith(".dll"))
            //    .Select(p => MetadataReference.CreateFromFile(p))
            //    .ToList();
            //compilation = compilation.AddReferences(referenceAssemblies);          

            ////DOMAIN ASSEMBLIES
            //var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            //    .Where(x => !x.IsDynamic)
            //    .Select(x => MetadataReference.CreateFromFile(x.Location))
            //    .ToList();
            //compilation = compilation.AddReferences(domainAssemblies);            

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    throw new Exception(result.ToString());
                }

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                try
                {
                    var types = assembly.GetTypes();
                }
                catch (Exception ex)
                {
                    throw;
                }


                dynamic task = assembly.CreateInstance("Consumer.MyTask");
                task.Run();
            }
        }
    }
}