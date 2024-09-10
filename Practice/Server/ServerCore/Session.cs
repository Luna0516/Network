using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Session
    {
        bool _pending = false;

        int _disconnected = 0;
        
        Socket _socket;

        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

        Queue<byte[]> _sendQueue = new Queue<byte[]>();

        object _lock = new object();

        public void Init(Socket socket)
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

                if(!_pending)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신
        private void RegisterSend()
        {
            _pending = true;

            byte[] buff = _sendQueue.Dequeue();
            _sendArgs.SetBuffer(buff, 0, buff.Length);

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
                        if (_sendQueue.Count > 0)
                            RegisterSend();
                        else
                            _pending = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {ex}");
                    }
                }
                else
                {
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
                    string recvData = Encoding.UTF8.GetString(_recvArgs.Buffer, _recvArgs.Offset, _recvArgs.BytesTransferred);
                    Console.WriteLine($"[From Client] : {recvData}");

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
