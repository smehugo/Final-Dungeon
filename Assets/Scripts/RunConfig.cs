// main menu option for a selected run
public static class RunConfig
{
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
    }
}