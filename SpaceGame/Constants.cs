namespace SpaceGame
{
    public static class ScaleType
    {
        public const float Half = 0.5f;
        public const float Full = 1f;
    }

    public enum FactionType
    {
        None,
        Player,
        Enemy
    }

    public enum TextSize
    {
        Small,
        Medium,
        Large
    }

    public class Constants
    {
        public const float LandingSpeed = 200f;
        public const int UniverseRadius = 1000;
        public const int NumberOfSystems = 100;
    }

    public static class SceneNames
    {
        public const string MainMenu = "Main Menu";
        public const string GameOver = "Game Over";
        public const string PauseMenu = "Pause Menu";
        public const string Space = "Space";
        public const string UniverseMap = "Universe Map";
    }
}
