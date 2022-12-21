using DynamicCompilationNetStandard2._0;

var source = """
    using System;
    using Core;

    namespace Consumer
    {
        public class MyTask : ITask
        {
            public void Run()
            {
                Console.WriteLine("Finished");
            }
        }
    }
    """;

Executor.Execute(source);

class SourceGenDummy
{ 
}