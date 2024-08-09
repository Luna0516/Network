
using System.Threading.Channels;

namespace ServerCore
{
    // 메모리 베리어
    // A) 코드 재배치 억제
    // B) 가시성
    
    // 1) Full Memory Barrier (ASM MFENCE, C# Thread.MemoryBarrier) : Store/Load 둘다 막는다.
    // 2) Store Memory Barrier (ASM SFENCE) : Store만 막는다.
    // 2) Load Memory Barrier (ASM LFENCE) : Load만 막는다.

    internal class Program
    {
        static int _answer;
        static bool _complete;

        static void A()
        {
            _answer = 123;
            Console.WriteLine("Barrier 1");
            Thread.MemoryBarrier(); // Barrier 1
            _complete = true;
            Console.WriteLine("Barrier 2");
            Thread.MemoryBarrier(); // Barrier 2
        }

        static void B()
        {
            Console.WriteLine("Barrier 3");
            Thread.MemoryBarrier(); // Barrier 3
            if(_complete)
            {
                Console.WriteLine("Barrier 4");
                Thread.MemoryBarrier(); // Barrier 4
                Console.WriteLine(_answer);
            }
        }

        static void Main(string[] args)
        {
            Task a = new Task(A);
            Task b = new Task(B);

            a.Start();
            b.Start();

            while(true)
            {

            }
        }
    }
}
