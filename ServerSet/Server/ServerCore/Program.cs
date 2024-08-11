using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Lock
    {
        // bool <- 커널
        // true : 연상태 // false : 닫힌 상태
        // 닫는 거도 자동으로 해준다
        // 커널을 통해 하기 때문에 오래 걸리는 단점
        // AutoResetEvent _available = new AutoResetEvent(true);

        // 락구현에서 잘못된 방법
        ManualResetEvent _available = new ManualResetEvent(true);

        public void Acquire()
        {
            _available.WaitOne(); // 입장 시도
            // _available.Reset(); // bool을 false로 해준다

            _available.Reset(); // Manual은 자동으로 닫히지 않음
        }

        public void Release()
        {
            _available.Set(); // true로 다시 돌려준다. flag -> true
        }
    }

    class Program
    {
        static readonly int forSize = 10000;

        static int _num = 0;
        static Lock _spinLock = new Lock();

        static void Thread_1()
        {
            for(int i = 0; i < forSize; i++)
            {
                _spinLock.Acquire();
                _num++;
                _spinLock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < forSize; i++)
            {
                _spinLock.Acquire();
                _num--;
                _spinLock.Release();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}
