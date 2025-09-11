using System;
using SocketIOClient;
using UnityEngine;

// 게임 시작 시 서버로부터 받는 데이터 구조
public class GameStartData
{
    public string room;
    public string player1;
    public string player2;
}

// 돌 위치 정보 데이터 구조
public class StoneData
{
    public int x;
    public int y;
    public string player;
}
public class MultiplayController
{
    private SocketIOUnity socket;
    
    private Action<Constants.MultiplayControllerState, string> _onMultiplayStateChanged;
    public Action<int> onBlockDataChanged;

    public MultiplayController(Action<Constants.MultiplayControllerState, string> onMultiplayStateChanged)
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

        socket.On("waitingForPlayer", (response) =>
        {
            var data = JsonUtility.FromJson<GameStartData>(response.GetValue().GetRawText());
            _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.Waiting, data.room);
            Debug.Log($"[게임 시작] 방 ID: {data.room}, 플레이어1: {data.player1}, 플레이어2: {data.player2}");
            // 여기에 게임판을 초기화하고 게임을 시작하는 로직을 추가
        });
        
        socket.On("second", (response) =>
        {
            var data = JsonUtility.FromJson<GameStartData>(response.GetValue().GetRawText());
            _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.Second, data.room);
            Debug.Log($"[게임 시작] 방 ID: {data.room}, 플레이어1: {data.player1}, 플레이어2: {data.player2}");
        });
        socket.On("gameStart", (response) =>
        {
            var data = JsonUtility.FromJson<GameStartData>(response.GetValue().GetRawText());
            _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.Start, data.room);
            Debug.Log($"[게임 시작] 방 ID: {data.room}, 플레이어1: {data.player1}, 플레이어2: {data.player2}");
        });

        socket.On("stonePlaced", StonePlaced);
        
        socket.Connect();
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