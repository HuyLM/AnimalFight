

/**<summary>Place ALL global game variables on this class.</summary>*/
public class GameData {
    public GameMode GameMode { get; private set; }
    public void SetGameMode(GameMode mode) {
        GameMode = mode;
    }
}
