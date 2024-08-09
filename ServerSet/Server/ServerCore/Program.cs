using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class SpinLock
    {
        //volatile bool _locked = false;

        int _locked = 0;

        public void Acquire()
        {
            //while (_locked)
            //{
            //    // 잠김 풀리기를 기다린다.
            //}

            //_locked = true;
            int expected = 0;
            int desired = 1;
            while (Interlocked.CompareExchange(ref _locked, desired, expected) != expected)
            {
                // 잠김 풀리기를 기다린다.
            }

            // 쉬는 포인트
            // Thread.Sleep(1); // 무조건 휴식 => 무조건 1ms 정도 쉬고 싶어요 (실제로 몇초 쉴껀지 운영체제의 스케줄러가 정해준다, 최대한 요청한 거랑 비슷하게 해준다.)
            // Thread.Sleep(0); // 조건부 양보 => 나보다 우선순위가 낮은 애들한테는 양보 불가 => 우선순위가 나보다 같거나 높은 쓰레드가 없으면 다시 본인한테
            Thread.Yield();  // 관대한 양보 => 관대하게 양보할테니, 지금 실행이 가능한 쓰레드가 있으면 실행하세요 => 실행 가능한 애가 없으면 남은 시간 소진
        }

        public void Release()
        {
            _locked = 0;
        }
    }

    class Program
    {
        static readonly int forSize = 10000;

        static int _num = 0;
        static SpinLock _spinLock = new SpinLock();

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
