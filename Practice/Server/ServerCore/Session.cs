using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class Session
    {
        int _disconnected = 0;
        
        Socket _socket;

        ReceiveBuffer _receiveBuffer = new ReceiveBuffer(1024);

        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        object _lock = new object();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract int OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += (_, _) => OnReceiveCompleted();
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed += (_, _) => OnSendCompleted();

            RegisterReceive();
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);

                if(_pendingList.Count == 0)
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
        }

        #region 네트워크 통신
        private void RegisterSend()
        {
            _pendingList.Clear();

            while (_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }
            
            _sendArgs.BufferList = _pendingList;
                
            if (!_socket.SendAsync(_sendArgs))
                OnSendCompleted();
        }

        private void OnSendCompleted()
        {
            lock (_lock)
            {
                if (_sendArgs.BytesTransferred > 0 && _sendArgs.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {ex}");
                    }
                }
                else
                {
                    Console.WriteLine($"Send failed with error: {_sendArgs.SocketError}");
                    Disconnect();
                }
            }
        }

        private void RegisterReceive()
        {
            _receiveBuffer.Clean();

            ArraySegment<byte> segment = _receiveBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            if(!_socket.ReceiveAsync(_recvArgs))
                OnReceiveCompleted();
        }

        private void OnReceiveCompleted()
        {
            if (_recvArgs.BytesTransferred > 0 && _recvArgs.SocketError == SocketError.Success)
            {
                try
                {
                    if(!_receiveBuffer.OnWrite(_recvArgs.BytesTransferred))
                    {
                        Disconnect();
                        return;
                    }

                    int processLength = OnReceive(_receiveBuffer.ReadSegment);
                    if (processLength < 0 || _receiveBuffer.DataSize < processLength)
                    {
                        Disconnect();
                        return;
                    }

                    if (!_receiveBuffer.OnRead(processLength))
                    {
                        Disconnect();
                        return;
                    }

                    RegisterReceive();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {ex}");
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
