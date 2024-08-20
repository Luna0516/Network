using Server;
using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            for (int i = 0; i < 5; i++)
            {
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World! {i}");
                Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnReceive(ArraySegment<byte> buffer)
        {
            try
            {
                // 역직렬화
                Knight knight = Knight.Deserialize(buffer.Array.Skip(buffer.Offset).Take(buffer.Count).ToArray());

                Console.WriteLine($"[From Server] Knight HP: {knight.Hp}, Attack: {knight.Attack}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error during deserialization: {e.Message}");
            }

            int processedLength = buffer.Count;
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, processedLength);
            Console.WriteLine($"[From Server] {recvData}");

            //int hp = BitConverter.ToInt32(buffer.Array, buffer.Offset);
            //int attack = BitConverter.ToInt32(buffer.Array, buffer.Offset + sizeof(int));

            //Console.WriteLine($"[From Server] Knight HP: {hp}, Attack: {attack}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return new GameSession(); });

            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {


                    break;
                }
            }

            Console.WriteLine("Client shutting down...");
        }
    }
}
