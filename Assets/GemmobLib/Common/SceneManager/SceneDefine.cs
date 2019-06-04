

public class SceneDefined {

    public class Name {
        public const string ScenePath = "Game/Scenes/";
        public const string Logo = ScenePath + "Logo";
        public const string Home = ScenePath + "Home";
        public const string Game = ScenePath + "CreationScene";
    }

    public enum Index : int {
        Logo = 0,
        Home = 1,
        GamePlay = 2,
    }
}
