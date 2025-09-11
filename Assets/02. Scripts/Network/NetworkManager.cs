using System;
using SocketIOClient;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager
{
    private SocketIOUnity socket;
    public string myRoomId;
    
    private Action<string> _onMultiplayStateChanged;
    public Action<int> onBlockDataChanged;

    public NetworkManager(Action<string> onMultiplayStateChanged)
    {
        _onMultiplayStateChanged = onMultiplayStateChanged;
        
        var uri = new System.Uri("http://localhost:3000");
        socket = new SocketIOUnity(uri);

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("서버에 연결되었습니다. 다른 플레이어를 기다립니다.");
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log($"서버 연결이 끊어졌습니다: {e}");
        };

        // 1. 서버로부터 게임 시작 메시지를 받을 때
        socket.On("gameStart", (response) =>
        {
            var data = JsonUtility.FromJson<GameStartData>(response.GetValue().GetRawText());
            myRoomId = data.room;
            Debug.Log($"[게임 시작] 방 ID: {myRoomId}, 플레이어1: {data.player1}, 플레이어2: {data.player2}");
            // 여기에 게임판을 초기화하고 게임을 시작하는 로직을 추가
        });

        // 2. 서버로부터 돌이 놓였다는 메시지를 받을 때
        socket.On("stonePlaced", StonePlaced);

        
        socket.Connect();

        // 3. 서버에 돌 위치 전송
        // 이 함수는 오목판을 클릭했을 때 호출될 것입니다.
        // 예를 들어, Input.GetMouseButtonDown(0)을 사용하여 클릭 감지
    }

    void Start()
    {
        var uri = new System.Uri("http://localhost:3000");
        socket = new SocketIOUnity(uri);

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("서버에 연결되었습니다. 다른 플레이어를 기다립니다.");
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log($"서버 연결이 끊어졌습니다: {e}");
        };

        // 1. 서버로부터 게임 시작 메시지를 받을 때
        socket.On("gameStart", (response) =>
        {
            var data = JsonUtility.FromJson<GameStartData>(response.GetValue().GetRawText());
            myRoomId = data.room;
            Debug.Log($"[게임 시작] 방 ID: {myRoomId}, 플레이어1: {data.player1}, 플레이어2: {data.player2}");
            // 여기에 게임판을 초기화하고 게임을 시작하는 로직을 추가
        });

        // 2. 서버로부터 돌이 놓였다는 메시지를 받을 때
        socket.On("stonePlaced", StonePlaced);

        
        socket.Connect();
        Debug.Log("NetWorking");

        // 3. 서버에 돌 위치 전송
        // 이 함수는 오목판을 클릭했을 때 호출될 것입니다.
        // 예를 들어, Input.GetMouseButtonDown(0)을 사용하여 클릭 감지
    }
    
    // 이 함수를 추가해 주세요.
    void OnApplicationQuit()
    {
        // 게임이 종료될 때 서버와의 연결을 끊습니다.
        socket?.Disconnect();
        Debug.Log("게임이 종료되었습니다. 서버 연결을 끊습니다.");
    }

    public void StonePlaced(SocketIOResponse response)
    {
        var data = JsonUtility.FromJson<StoneData>(response.GetValue().GetRawText());
        Debug.Log($"서버로부터 돌 위치 받음: ({data.x}, {data.y})");
        onBlockDataChanged?.Invoke(data.y * Constants.BoardSize + data.x);
    }

    public void PlaceStone(string myRoomId, int x, int y)
    {
        socket.Emit("placeStone", new { room = myRoomId, x = x, y = y });
    }

    // 외부에서 호출될 함수: 오목판을 클릭했을 때
    public void PlaceStone(int x, int y)
    {
        if (string.IsNullOrEmpty(myRoomId))
        {
            Debug.Log("아직 방에 입장하지 않았습니다.");
            return;
        }
        
        // 서버에 돌 위치 정보 전송
        socket.Emit("placeStone", new { room = myRoomId, x = x, y = y });
    }

    private void PlaceStoneOnBoard(int x, int y)
    {
        // 여기에 실제로 오목알 프리팹을 생성하고 위치시키는 로직을 구현
        Debug.Log($"오목알을 ({x}, {y})에 그립니다.");
    }
    
    // 게임 시작 시 서버로부터 받는 데이터 구조
    [System.Serializable]
    private class GameStartData
    {
        public string room;
        public string player1;
        public string player2;
    }

    // 돌 위치 정보 데이터 구조
    [System.Serializable]
    private class StoneData
    {
        public int x;
        public int y;
        public string player;
    }

    public void Dispose()
    {
        if (socket != null)
        {
            socket.Disconnect();
            socket.Dispose();
            socket = null;
        }
    }
}