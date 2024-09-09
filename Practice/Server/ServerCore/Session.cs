using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Session
    {
        Socket _socket; // 클라이언트와의 연결을 위한 소켓

        int _disconnected = 0; // 연결 해제 상태를 나타내는 플래그

        object _lock = new object(); // 동기화를 위한 잠금 객체
        Queue<byte[]> _sendQueue = new Queue<byte[]>(); // 송신할 데이터 버퍼를 큐에 저장
        bool _pending = false; // 송신 작업이 대기 중인지 여부를 나타내는 플래그
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs(); // 송신을 위한 비동기 작업 인자

        public void Start(Socket socket)
        {
            _socket = socket;

            // 수신을 위한 SocketAsyncEventArgs 객체 생성 및 설정
            SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += OnReceiveCompleted;
            receiveArgs.SetBuffer(new byte[1024], 0, 1024); // 1024 바이트 버퍼 설정

            _sendArgs.Completed += OnSendCompleted; // 송신 완료 이벤트 핸들러 등록
            RegisterReceive(receiveArgs); // 비동기 수신 등록
        }

        public void Send(byte[] sendBuffer)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuffer); // 송신할 데이터를 큐에 추가

                if (!_pending) // 송신 작업이 대기 중이지 않으면 송신 등록
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            // 이미 연결이 해제된 경우 아무 작업도 하지 않음
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            // 소켓의 양쪽 방향으로 연결 종료 및 소켓 닫기
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신

        /// <summary>
        /// 송신 작업을 비동기적으로 등록하는 메서드
        /// </summary>
        private void RegisterSend()
        {
            _pending = true; // 송신 작업이 대기 중임을 표시

            byte[] buffer = _sendQueue.Dequeue(); // 큐에서 송신할 데이터 버퍼를 가져옴
            _sendArgs.SetBuffer(buffer, 0, buffer.Length); // 송신 버퍼 설정

            // 비동기 송신 요청 (오류: 실제로는 SendAsync를 호출해야 함)
            bool pending = _socket.ReceiveAsync(_sendArgs);
            if (!pending)
                OnSendCompleted(null, _sendArgs); // 즉시 완료된 경우 핸들러 호출
        }

        /// <summary>
        /// 송신 완료 시 호출되는 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        if (_sendQueue.Count != 0)
                            RegisterSend(); // 큐에 데이터가 있으면 계속 송신 등록
                        else
                            _pending = false; // 큐가 비어 있으면 송신 작업 종료
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect(); // 오류 발생 시 연결 종료
                }
            }
        }

        /// <summary>
        /// 수신 작업을 비동기적으로 등록하는 메서드
        /// </summary>
        /// <param name="args"></param>
        private void RegisterReceive(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args); // 비동기 수신 요청
            if (!pending)
                OnReceiveCompleted(null, args); // 즉시 완료된 경우 핸들러 호출
        }

        /// <summary>
        /// 수신 완료 시 호출되는 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // 수신한 데이터를 문자열로 변환하여 출력
                    string receiveData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {receiveData}");

                    RegisterReceive(args); // 계속해서 수신 등록
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OnReceiveCompleted Failed : {ex}");
                }
            }
            else
            {
                Disconnect(); // 오류 발생 시 연결 종료
            }
        }

        #endregion
    }
}
