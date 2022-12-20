using System;
using System.Linq.Expressions;

namespace Core
{
    public interface ITask
    {
        void CanRun<T>(Expression<Func<T, bool>> predicate);
        void Run();
    }
}