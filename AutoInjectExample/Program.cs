using AutoInjectExample.Services;
using System;

namespace AutoInjectExample
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("AutoInjectCtor example run...");
            var a = new ServiceA();
            var b = new ServiceB();
            var c = new ServiceC();

            var worker1 = new Worker(a, b, c);
            worker1.Execute();

            Console.ReadKey();
        }
    }
}
