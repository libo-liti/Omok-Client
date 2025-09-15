
public static class Constants
{
    public const string ServerURL = "http://localhost:3000/";
    public enum MultiplayControllerState
    {
        CreateRoom,
        JoinRoom,
        GameStart,
        EndGame
    }
    public enum GameType { SinglePlay, DualPlay, MultiPlay }
    public enum PlayerType { None, PlayerA, PlayerB }

    public const int BoardSize = 15;
}
