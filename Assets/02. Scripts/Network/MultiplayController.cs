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
        
        var uri = new System.Uri(Constants.ServerURL);
        socket = new SocketIOUnity(uri);

        socket.OnConnected += OnServerConnected;
        socket.OnDisconnected += OnServerDisconnected;

        socket.On("createRoom", CreateRoom);
        socket.On("joinRoom", JoinRoom);
        socket.On("gameStart", GameStart);
        socket.On("doOpponent", DoOpponent);
        
        socket.Connect();
    }

    private void OnServerConnected(object sender, EventArgs e)
    {
        Debug.Log("서버에 연결되었습니다");
    }

    private void OnServerDisconnected(object sender, string e)
    {
        Debug.Log($"서버 연결이 끊어졌습니다: {e}");
    }

    private void CreateRoom(SocketIOResponse response)
    {
        var data = JsonUtility.FromJson<GameStartData>(response.GetValue().GetRawText());
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.CreateRoom, data.room);
    }

    private void JoinRoom(SocketIOResponse response)
    {
        var data = JsonUtility.FromJson<GameStartData>(response.GetValue().GetRawText());
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.JoinRoom, data.room);
    }

    private void GameStart(SocketIOResponse response)
    {
        var data = JsonUtility.FromJson<GameStartData>(response.GetValue().GetRawText());
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.GameStart, data.room);
        Debug.Log($"[게임 시작] 방 ID: {data.room}, 플레이어1: {data.player1}, 플레이어2: {data.player2}");
    }
    
    public void DoOpponent(SocketIOResponse response)
    {
        var data = JsonUtility.FromJson<StoneData>(response.GetValue().GetRawText());
        onBlockDataChanged?.Invoke(data.y * Constants.BoardSize + data.x);
    }

    public void DoPlayer(string myRoomId, int x, int y)
    {
        socket.Emit("doPlayer", new { room = myRoomId, x = x, y = y });
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