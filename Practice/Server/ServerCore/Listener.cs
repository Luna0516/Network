using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;

        Action<Socket> _onAcceptHandler;

        SocketAsyncEventArgs _args = new SocketAsyncEventArgs();

        public void Init(EndPoint endPoint, Action<Socket> onAcceptHandler, int listener = 10)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;

            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(listener);

            _args.Completed += (_, _) => OnAcceptCompleted();
            RegisterAccept();
        }

        private void RegisterAccept()
        {
            _args.AcceptSocket = null;

            if (!_listenSocket.AcceptAsync(_args))
                OnAcceptCompleted();
        }

        private void OnAcceptCompleted()
        {
            if(_args.SocketError == SocketError.Success)
            {
                _onAcceptHandler.Invoke(_args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(_args.SocketError.ToString());
            }

            RegisterAccept();
        }
    }
}
