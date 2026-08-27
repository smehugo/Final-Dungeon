// main menu option for a selected run
public static class RunConfig
{
    public static bool UseCustomGen = false;
    public static int MapSize = 128;
    public static int RoomCount = 16;
    public static int Artifacts = 5;
    public static float RoomFill = 0.8f;

    public static bool UseFixedSeed = false;
    public static int Seed = 1;

    // added to enemy SO room count for extra difficulty
    public static int EnemyCountBonus = 0;
    public static string DiffName = "Normal";

    public static void Reset()
    {
        UseFixedSeed = false;
        Seed = 1;
        EnemyCountBonus = 0;
        DiffName = "Normal";
        UseCustomGen = false;
        MapSize = 128;
        RoomCount = 16;
        Artifacts = 5;
        RoomFill = 0.8f;
    }
}