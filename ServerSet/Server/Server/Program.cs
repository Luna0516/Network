using System.Net;
using System.Text;
using ServerCore;

namespace Server
{
    public class Knight
    {
        public int Hp { get; set; }
        public int Attack { get; set; }

        public Knight(int hp, int attack) 
        {
            Hp = hp;
            Attack = attack;
        }

        // Serialize the Knight object to a byte array
        public byte[] Serialize()
        {
            byte[] hpBytes = BitConverter.GetBytes(Hp);
            byte[] attackBytes = BitConverter.GetBytes(Attack);

            byte[] result = new byte[hpBytes.Length + attackBytes.Length];
            Array.Copy(hpBytes, 0, result, 0, hpBytes.Length);
            Array.Copy(attackBytes, 0, result, hpBytes.Length, attackBytes.Length);

            return result;
        }

        // Deserialize a byte array to create a Knight object
        public static Knight Deserialize(byte[] data)
        {
            if (data.Length < 8) // Must have at least 8 bytes (2 int values)
            {
                throw new ArgumentException("Data is too short to deserialize");
            }

            int hp = BitConverter.ToInt32(data, 0);
            int attack = BitConverter.ToInt32(data, 4);

            return new Knight(hp, attack);
        }
    }

    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Knight knight = new Knight(10, 100);
            byte[] serializedData = knight.Serialize();
            ArraySegment<byte> sendBuff = new ArraySegment<byte>(serializedData);

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(knight._hp);
            //byte[] buffer2 = BitConverter.GetBytes(knight._attak);
            //Array.Copy(buffer, 0, openSegment.Array, 0, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
            
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnReceive(ArraySegment<byte> buffer)
        {
            int processedLength = buffer.Count; // 또는 실제로 처리한 데이터의 길이
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, processedLength);
            Console.WriteLine($"[From Client] {recvData}");

            return processedLength;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }

    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            Console.WriteLine("Server shutting down...");
        }
    }
}
