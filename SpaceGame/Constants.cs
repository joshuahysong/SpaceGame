namespace SpaceGame
{
    public enum GameState
    {
        MainMenu,
        Space
    }

    public static class ScaleType
    {
        public const float Half = 0.5f;
        public const float Full = 1f;
    }

    public enum FactionType
    {
        None = 0,
        Player = 1,
        Enemy = 2
    }
}
