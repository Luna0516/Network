using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;

        Func<Session> _sessionFactory;

        SocketAsyncEventArgs _args = new SocketAsyncEventArgs();

        public void Init(EndPoint endPoint, Func<Session> sessionFactory, int listener = 10)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

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
                Session session = _sessionFactory.Invoke();
                session.Start(_args.AcceptSocket);
                session.OnConnected(_args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(_args.SocketError.ToString());
            }

            RegisterAccept();
        }
    }
}
