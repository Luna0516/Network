using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    class Listener
    {
        readonly int _listenerSize = 10; // 동시에 처리할 수 있는 비동기 accept 작업의 개수

        Socket _listenSocket; // 수신 소켓

        Action<Socket> _onAcceptHandler; // 클라이언트 소켓이 연결되었을 때 호출되는 델리게이트

        /// <summary>
        /// 초기화 메서드, 수신 소켓을 설정하고 accept 작업을 비동기적으로 등록
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="onAcceptHandler"></param>
        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            // 수신 소켓을 생성
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler; // 클라이언트 연결 시 호출할 핸들러 등록

            // 소켓을 엔드포인트에 바인딩하고 연결 요청을 수신 대기
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);

            // 비동기 accept 작업을 여러 개 등록
            for (int i = 0; i < _listenerSize; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnAcceptCompleted; // accept 작업 완료 시 호출할 이벤트 핸들러 등록
                RegisterAccept(args); // 비동기 accept 작업 등록
            }
        }

        /// <summary>
        /// 비동기 accept 작업을 등록하는 메서드
        /// </summary>
        /// <param name="args"></param>
        private void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null; // 기존의 acceptSocket을 null로 초기화

            // 비동기 accept 요청
            bool pending = _listenSocket.AcceptAsync(args);
            if (!pending) // 즉시 완료된 경우 핸들러를 직접 호출
                OnAcceptCompleted(null, args);
        }

        /// <summary>
        /// 비동기 accept 작업이 완료되었을 때 호출되는 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                // 클라이언트 소켓이 성공적으로 연결된 경우 핸들러를 호출
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
            {
                // 오류 발생 시 오류 메시지 출력
                Console.WriteLine(args.SocketError.ToString());
            }

            // 새로운 비동기 accept 작업을 등록하여 계속해서 클라이언트 연결을 수신 대기
            RegisterAccept(args);
        }
    }
}
