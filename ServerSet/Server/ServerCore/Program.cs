using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // 1. 근성
    // 2. 양보
    // 3. 갑질

    class Program
    {
        // 상호 배제
        // Monitor
        static object _lock = new object();
        static SpinLock _spinLock = new SpinLock();
        static Mutex _mutex = new Mutex(); // 사용X
        // 직접 만든다

        // [ ] [ ] [ ] [ ] [ ]
        class Reward
        {

        }

        // RWLock ReaderWriterLock
        static ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        // 99.99999 - 0.00001
        static Reward GetReward(int id)
        {
            _readerWriterLockSlim.EnterReadLock();
            
            _readerWriterLockSlim.ExitReadLock();

            //lock (_lock)
            //{
                
            //}

            return null;
        }

        static void  AddReward(Reward reward)
        {
            lock(_lock)
            {

            }
        }

        static void Main(string[] args)
        {
            lock (_lock)
            {

            }

            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
            }
            finally
            {
                if(lockTaken)
                    _spinLock.Exit();
            }
        }
    }
}
