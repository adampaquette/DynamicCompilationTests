using DynamicCompilationNetStandard2._0;

var source = """
    using System.Linq.Expressions;

    namespace Consumer.Tasks;

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
    """;

Executor.Execute(source);