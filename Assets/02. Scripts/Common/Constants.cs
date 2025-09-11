
public static class Constants
{
    public enum MultiplayControllerState
    {
        CreateRoom,
        JoinRoom,
        StartGame,
        ExitRoom,
        EndGame
    }
    public enum GameType { SinglePlay, DualPlay, MultiPlay }
    public enum PlayerType { None, PlayerA, PlayerB }

    public const int BoardSize = 15;
}
