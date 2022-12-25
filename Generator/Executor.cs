using Basic.Reference.Assemblies;
using Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;

namespace Generator
{
    public static class Executor
    {
        public static void Execute(string source)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var syntaxTree = SyntaxFactory.ParseSyntaxTree(source);
            var compilation = CSharpCompilation.Create(assemblyName: Path.GetRandomFileName())
                .WithReferenceAssemblies(ReferenceAssemblyKind.NetStandard20)
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(ITask).Assembly.Location))
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(syntaxTree);

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

        private static Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) =>
            args.Name == "Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                ? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == args.Name)
                : null;
    }
}
