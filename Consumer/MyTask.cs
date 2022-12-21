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