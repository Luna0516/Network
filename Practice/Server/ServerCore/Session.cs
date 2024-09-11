using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class Session
    {
        int _disconnected = 0;
        
        Socket _socket;

        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

        Queue<byte[]> _sendQueue = new Queue<byte[]>();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        object _lock = new object();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract void OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += (_, _) => OnRecvCompleted();
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed += (_, _) => OnSendCompleted();

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
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
                byte[] buff = _sendQueue.Dequeue();
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
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

        private void RegisterRecv()
        {
            if(!_socket.ReceiveAsync(_recvArgs))
                OnRecvCompleted();
        }

        private void OnRecvCompleted()
        {
            if (_recvArgs.BytesTransferred > 0 && _recvArgs.SocketError == SocketError.Success)
            {
                try
                {
                    OnReceive(new ArraySegment<byte>(_recvArgs.Buffer, _recvArgs.Offset, _recvArgs.BytesTransferred));

                    RegisterRecv();
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
