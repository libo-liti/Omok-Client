using System;
using System.Collections.Generic;
using SocketIOClient;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class RoomData {
    public string roomName;
    public string mode;
}
[Serializable]
public class RoomList
{
    public RoomData[] rooms;
}
public class NetworkManager : Singleton<NetworkManager>
{
    private List<GameObject> roomPrefabList = new List<GameObject>();
    private SocketIOUnity socket;
    [SerializeField] private GameObject roomPrefab;
    public Transform roomListContent;
    public Button closedButton;
    
    public Constants.MultiplayControllerState state;
    public string roomName;
    public string mode;
    
    public void Init()
    {
        var uri = new System.Uri(Constants.ServerURL);
        socket = new SocketIOUnity(uri);

        socket.OnConnected += OnServerConnected;
        socket.OnDisconnected += OnServerDisconnected;

        socket.OnUnityThread("roomsList", RoomsList);
        socket.OnUnityThread("createRoomSuccess", CreateRoomSuccess);
        socket.OnUnityThread("createRoomFailed", CreateRoomFailed);
        socket.OnUnityThread("joinRoomSuccess", JoinRoomSuccess);
        socket.OnUnityThread("joinRoomFailed", JoinRoomFailed);
        socket.Connect();
    }
    
    private void OnServerConnected(object sender, EventArgs e)
    {
         Debug.Log("서버에 연결되었습니다");
        GetRooms();
    }

    private void OnServerDisconnected(object sender, string e)
    {
        Debug.Log($"서버 연결이 끊어졌습니다: {e}");
        UnityThread.executeInUpdate(() =>
        {
            if (mode == "normal")
            {
                GameManager.Instance.ChangeToGameScene(Constants.GameType.MultiPlay);
            }
            else if (mode == "arcade")
            {
                GameManager.Instance.ChangeToGameScene(Constants.GameType.ArcadePlay);
            }
        });
    }
    
    // 멀티방 목록 가져오기
    private void RoomsList(SocketIOResponse response)
    {
        var data = JsonUtility.FromJson<RoomList>(response.GetValue().GetRawText());

        foreach (var room in roomPrefabList)
            Destroy(room);
        roomPrefabList.Clear();
        
        foreach (var room in data.rooms)
        {
            var obj = Instantiate(roomPrefab, roomListContent);
            obj.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = room.roomName;
            obj.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = room.mode;
            roomPrefabList.Add(obj);
        }
    }

    private void CreateRoomSuccess(SocketIOResponse response)
    {
        Debug.Log("방 만들 수 있음");
        var data = JsonUtility.FromJson<RoomData>(response.GetValue().GetRawText());
        state = Constants.MultiplayControllerState.CreateRoom;
        roomName = data.roomName;
        mode = data.mode;
        socket.Disconnect();
    }

    private void CreateRoomFailed(SocketIOResponse response)
    {
        GameManager.Instance.OpenConfirmPanel("이미 같은 이름의 방이 있습니다.", null);
    }

    private void JoinRoomSuccess(SocketIOResponse response)
    {
        Debug.Log("방 참가 가능");
        var data = JsonUtility.FromJson<RoomData>(response.GetValue().GetRawText());
        state = Constants.MultiplayControllerState.JoinRoom;
        roomName = data.roomName;
        mode = data.mode;
        socket.Disconnect();
    }

    private void JoinRoomFailed(SocketIOResponse response)
    {
        GameManager.Instance.OpenConfirmPanel("방이 존재하지 않거나 가득 찼습니다.", null);
    }
    
    // 멀티방 목록 요청
    public void GetRooms()
    {
        socket.Emit("getRooms");
    }

    // 멀티방 만들기
    public void CreateRoomCheck(string roomName, string mode)
    {
        socket.Emit("createRoomCheck", new {roomName, mode});
    }
    // 입장 가능한지 체크
    public void JoinRoomCheck(string roomName, string mode)
    {
        socket.Emit("joinRoomCheck", new {roomName = roomName, mode = mode});
    }

    public void Dispose()
    {
        socket?.Disconnect();
        socket?.Dispose();
        socket = null;
    }
    
    private void OnApplicationQuit()
    {
        socket?.Disconnect();
        socket?.Dispose();
        socket = null;
    }

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            roomName = null;
            this.mode = null;
        }
    }
}
