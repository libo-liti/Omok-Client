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

public class EmojiData
{
    public int emoji;
}
public class MultiplayController
{
    private SocketIOUnity socket;
    private string _roomId;
    
    public Action<Constants.MultiplayControllerState, string> _onMultiplayStateChanged;
    public Action<int> onBlockDataChanged;
    public Action<int> setEmoji;
    public Action onDisconnectComplete;
    
    public MultiplayController(Action<Constants.MultiplayControllerState, string> onMultiplayStateChanged)
    {
        _onMultiplayStateChanged = onMultiplayStateChanged;
        
        var uri = new System.Uri(Constants.ServerURL);
        socket = new SocketIOUnity(uri);

        socket.OnConnected += OnServerConnected;
        socket.OnDisconnected += OnServerDisconnected;

        socket.OnUnityThread("createRoom", CreateRoom);
        socket.OnUnityThread("joinRoom", JoinRoom);
        socket.OnUnityThread("gameStart", GameStart);
        socket.OnUnityThread("doOpponent", DoOpponent);
        socket.OnUnityThread("opponentEmoji", OpponentEmoji);
        socket.OnUnityThread("escapeOpponent", EscapeOpponent);
        
        socket.Connect();
    }

    private void OnServerConnected(object sender, EventArgs e)
    {
        Debug.Log("서버에 연결되었습니다");
        string nickname = GameManager.Instance.guestName;
        socket.Emit("registerNickname", new {nickname = nickname});
    }

    private void OnServerDisconnected(object sender, string e)
    {
        UnityThread.executeInUpdate(() =>
        {
            Debug.Log($"서버 연결이 끊어졌습니다: {e}");
            socket.Dispose();
            socket = null;
            onDisconnectComplete?.Invoke();
        });
    }

    private void CreateRoom(SocketIOResponse response)
    {
        var data = JsonUtility.FromJson<GameStartData>(response.GetValue().GetRawText());
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.CreateRoom, data.room);
        _roomId = data.room;
        GameSceneUIManager.Instance.SurrenderAction = () =>
        {
            socket.Emit("surrender", new { room = data.room });
            _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.ExitRoom, data.room);
        };
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
        GameSceneUIManager.Instance.SurrenderAction = () =>
        {
            socket.Emit("surrender", new { room = data.room });
            _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.ExitRoom, data.room);
        };
        GameSceneUIManager.Instance.ExitRoomAction = () =>
        {
            _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.ExitRoom, _roomId);
        };
    }
    
    private void DoOpponent(SocketIOResponse response)
    {
        var data = JsonUtility.FromJson<StoneData>(response.GetValue().GetRawText());
        onBlockDataChanged?.Invoke(data.y * Constants.BoardSize + data.x);
    }
    
    private void OpponentEmoji(SocketIOResponse response)
    {
        var data = JsonUtility.FromJson<EmojiData>(response.GetValue().GetRawText());
        setEmoji?.Invoke(data.emoji);
    }

    private void EscapeOpponent(SocketIOResponse response)
    {
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayControllerState.EndGame, _roomId);
    }

    public void DoPlayer(string myRoomId, int x, int y)
    {
        socket.Emit("doPlayer", new { room = myRoomId, x = x, y = y });
    }
    
    public void PlayerEmoji(string myRoomId, int emojiNum)
    {
        socket.Emit("playerEmoji", new { room = myRoomId, emoji = emojiNum });
    }

    public void Dispose(Action onComplete)
    {
        onDisconnectComplete = onComplete;
        if(socket != null && socket.Connected)
            socket.Disconnect();
        else
            onDisconnectComplete?.Invoke();
    }

    public void Dispose()
    {
        onDisconnectComplete = null;
        if(socket != null && socket.Connected)
            socket.Disconnect();
    }
}