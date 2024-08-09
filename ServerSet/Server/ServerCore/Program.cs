using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class Program
    {
        static int forSize = 100000;

        static int number = 0;

        static object lockObj = new object();

        // 데드락 (DeadLock)
        // Monitor.Enter(lockObj); 잠그기
        // Monitor.Exit(lockObj);  잠금 풀기
        // 코드가 더러워 진다 - 가시성 으엑

        static void Thread_1()
        {
            for (int i = 0; i < forSize; i++)
            {
                lock (lockObj) 
                { 
                    number++; 
                }
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < forSize; i++)
            {
                lock (lockObj)
                {
                    number--;
                }
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
