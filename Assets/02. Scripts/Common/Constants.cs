
public static class Constants
{
    public const string ServerURL = "http://localhost:3000/";
    // public const string ServerURL = "http://3.85.87.205:3000/";
    public enum MultiplayControllerState
    {
        CreateRoom,
        JoinRoom,
        GameStart,
        EndGame,
        ExitRoom
    }
    public enum GameType { None ,SinglePlay, DualPlay, MultiPlay, ArcadePlay }
    public enum PlayerType { None, PlayerA, PlayerB }

    public const int BoardSize = 15;
}
