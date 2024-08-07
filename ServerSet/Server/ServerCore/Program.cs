
namespace ServerCore
{
    internal class Program
    {
        // 부담이 크다
        private static void MainThread()
        {
            while (true)
                Console.WriteLine("Hello Thread!");
        }

        // 단기 알바용 - ThreadPooling
        private static void SubThread(object? state)
        {
            for(int i = 0; i < 10; i++)
                Console.WriteLine("Hello SubThread!");
        }

        static void Main(string[] args)
        {
            // ThreadPool 실습
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            for (int i = 0; i < 5; i++)
            {
                // 좀 효율적으로 관리한다.
                Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning);
                // TaskCreationOptions.LongRunning = 오래 걸리는 작업이다 라고 알려준다.
                t.Start();
            }

            // ThreadPool을 5개를 다 사용하면 전체 시스템을 먹통으로 만들게 한다. => 해결법 : Task
            // for (int i = 0; i < 4; i++)
            //     ThreadPool.QueueUserWorkItem((obj) => { while (true) {  } });

            ThreadPool.QueueUserWorkItem(SubThread);

            //Thread t = new Thread(MainThread);
            //t.Name = "Test Thread";
            //t.IsBackground = true;
            //t.Start();
            //Console.WriteLine("Waitomg for Thread!");
            //t.Join();
            //Console.WriteLine("Hello World!");

            while(true)
            {

            }
        }
    }
}
