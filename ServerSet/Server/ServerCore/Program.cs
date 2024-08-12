using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        // TLS
        static ThreadLocal<string> ThreadName_TLS = new ThreadLocal<string>(() => { return $"My Name Is {Thread.CurrentThread.ManagedThreadId}"; });

        // 
        static string ThreadName_s = "";

        static void WhoAmI_TLS()
        {
            bool repeat = ThreadName_TLS.IsValueCreated;

            if (repeat)
                Console.WriteLine(ThreadName_TLS.Value + " (repeat)");
            else
                Console.WriteLine(ThreadName_TLS.Value);
        }

        static void WhoAmI_s()
        {
            ThreadName_s = $"My Name Is {Thread.CurrentThread.ManagedThreadId}";

            Thread.Sleep(1000);

            Console.WriteLine(ThreadName_s);
        }

        static void Main(string[] args)
        {
            Parallel.Invoke(WhoAmI_s, WhoAmI_s, WhoAmI_s, WhoAmI_s, WhoAmI_s, WhoAmI_s);

            Console.WriteLine("-----------------------------------------");

            Thread.Sleep(1000);

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);

            Parallel.Invoke(WhoAmI_TLS, WhoAmI_TLS, WhoAmI_TLS, WhoAmI_TLS, WhoAmI_TLS, WhoAmI_TLS);

            ThreadName_TLS.Dispose();
        }
    }
}
