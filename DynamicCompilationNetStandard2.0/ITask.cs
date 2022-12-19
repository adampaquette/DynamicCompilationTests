using System;
using System.Linq.Expressions;

namespace DynamicCompilationNetStandard2._0
{
    internal interface ITask
    {
        void CanRun<T>(Expression<Func<T, bool>> predicate);
        void Run();
    }
}