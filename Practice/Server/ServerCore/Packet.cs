namespace ServerCore
{
    public class Packet
    {
        public ushort Size { get; set; }
        public ushort PacketId { get; set; }

        public byte[] Serialize()
        {
            byte[] sizeBytes = BitConverter.GetBytes(Size);
            byte[] packetIdBytes = BitConverter.GetBytes(PacketId);

            byte[] result = new byte[sizeBytes.Length + packetIdBytes.Length];
            Array.Copy(sizeBytes, 0, result, 0, sizeBytes.Length);
            Array.Copy(packetIdBytes, 0, result, sizeBytes.Length, packetIdBytes.Length);

            return result;
        }

        public static Packet Deserialize(byte[] data)
        {
            if (data.Length < 4)
            {
                throw new ArgumentException("Data is too short to deserialize");
            }

            ushort size = BitConverter.ToUInt16(data, 0);
            ushort packetId = BitConverter.ToUInt16(data, 2);

            return new Packet { Size = size, PacketId = packetId };
        }
    }
}
