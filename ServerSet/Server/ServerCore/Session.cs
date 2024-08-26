using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;

        // [size(2)][packetId(2)][...] [size(2)][packetId(2)][...] ...
        public sealed override int OnReceive(ArraySegment<byte> buffer)
        {
            int processLenght = 0;
            int packetCount = 0;

            while (true)
            {
                // 최소한 Header는 파싱할 수 있는지 확인
                if (buffer.Count < HeaderSize)
                    break;

                // 패킷이 완전체로 도착했는지 확인
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                // 여기까지 왔으면 패킷 조립 가능
                // struct라서 힙 영역에 할당X , 스택에 복사를 하는 개념
                OnReceivePacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;

                processLenght += dataSize;
                buffer = new ArraySegment<byte> (buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            if(packetCount > 1)
                Console.WriteLine($"패킷 모아보내기 : {packetCount}");

            return processLenght;
        }

        public abstract void OnReceivePacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;

        int _disconnected = 0;

        ReceiveBuffer _receiveBuffer = new ReceiveBuffer(65535);

        object _lock = new object();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract int OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);


        void Clear() 
        {
            lock (_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }


        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterReceive();
        }

        public void Send(List<ArraySegment<byte>> sendBuffList)
        {
            if (sendBuffList.Count == 0)
                return;

            lock (_lock)
            {
                foreach(ArraySegment<byte> sendBuff in sendBuffList)
                    _sendQueue.Enqueue(sendBuff);

                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);

                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            OnDisconnected(_socket.RemoteEndPoint);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            Clear();
        }

        #region 네트워크 통신
        void RegisterSend()
        {
            if (_disconnected == 1)
                return;

            _pendingList.Clear();

            while (_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }

            _sendArgs.BufferList = _pendingList;

            try
            {
                // SendAsync를 통해 비동기적으로 데이터를 전송
                bool pending = _socket.SendAsync(_sendArgs);
                if (pending == false)
                    OnSendCompleted(null, _sendArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterSend Failed {e}");
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Console.WriteLine($"Send failed with error: {args.SocketError}");
                    Disconnect();
                }
            }
        }

        void RegisterReceive() 
        {
            if (_disconnected == 1)
                return;

            // 데이터 정리
            _receiveBuffer.Clean();

            ArraySegment<byte> segment = _receiveBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false)
                    OnReceiveCompleted(null, _recvArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterReceive Failed {e}");
            }
        }

        void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // Write 커서 이동
                    if(_receiveBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다.
                    int processLength = OnReceive(_receiveBuffer.ReadSegment);
                    if (processLength < 0 || _receiveBuffer.DataSize < processLength)
                    {
                        Disconnect();
                        return;
                    }

                    // Read 커서 이동
                    if(_receiveBuffer.OnRead(processLength) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterReceive();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnReceiveCompleted Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}
