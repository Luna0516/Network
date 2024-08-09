using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class Program
    {
        // 경합 조건 (Race Condition)
        // 원자적으로 처리를 해야 한다
        // Interlocked 로 처리해야 한다.

        // 다른 방법
        // static object lockObj = new object();
        // lock (lockObj) { number++ or --;}

        static int forSize = 100000;

        static int number = 0;
        
        static void Thread_1()
        {
            for (int i = 0; i < forSize; i++)
            {
                // All or Nothing
                Interlocked.Increment(ref number);
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < forSize; i++)
            {
                Interlocked.Decrement(ref number);
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}
