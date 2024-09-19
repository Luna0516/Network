using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServerCore
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

        public byte[] Serialize()
        {
            byte[] hpBytes = BitConverter.GetBytes(Hp);
            byte[] attackBytes = BitConverter.GetBytes(Attack);

            byte[] result = new byte[hpBytes.Length + attackBytes.Length];
            Array.Copy(hpBytes, 0, result, 0, hpBytes.Length);
            Array.Copy(attackBytes, 0, result, hpBytes.Length, attackBytes.Length);

            return result;
        }

        public static Knight Deserialize(byte[] data)
        {
            if (data.Length < 8)
            {
                Console.WriteLine("Data is too short to Deserialize");
                return null;
            }

            int hp = BitConverter.ToInt32(data, 0);
            int attack = BitConverter.ToInt32(data, 4);

            return new Knight(hp, attack);
        }
    }
}
