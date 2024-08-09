using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // 데드락 테스트

    class SessionManager
    {
        static object _lock = new object();

        public static void TestSession()
        {
            lock (_lock)
            {

            }
        }

        public static void Test()
        {
            lock (_lock)
            {
                UserManager.TestUser();
            }
        }
    }

    class UserManager
    {
        static object _lock = new object();

        public static void TestUser()
        {
            lock (_lock)
            {

            }
        }

        public static void Test()
        {
            lock (_lock)
            {
                SessionManager.TestSession();
            }
        }
    }

    internal class Program
    {
        static int forSize = 10000;

        static int number = 0;

        static object lockObj = new object();

        static void Thread_1()
        {
            for (int i = 0; i < forSize; i++)
            {
                SessionManager.Test();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < forSize; i++)
            {
                UserManager.Test();
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
