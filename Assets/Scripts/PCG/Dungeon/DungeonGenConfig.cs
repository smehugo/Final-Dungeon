// pcg config
// inspector override
public static class DungeonGenConfig
{
    public const int MapWidth = 128;
    public const int MapHeight = 128;
    public const int RoomPadding = 1;
    public const int RoomCount = 16;

    public const int MinRoomSize = 24;
    public const int MaxDepth = 4;
    public const float RoomFillMin = 0.8f;
    public const float RoomFillMax = 1f;

    public const int MinZoneSize = 4;
    public const int MaxZoneSize = 12;
    public const int InteriorDepthStep = 6;
    public const int InteriorMaxDepth = 10;
    public const int WallOpeningMin = 3;
    public const float WallOpeningMax = 0.3f;
    public const float WallExtraHoleGamba = 0.04f;
    public const int ArtifactZones = 5;

    public static int MinLeafSize => MinRoomSize + 2 * RoomPadding;

    public static int[] TestSeeds =
    {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
        11, 12, 13, 14, 15, 16, 17, 18, 19, 20
    };

    public const int DifficultyTiers = 5;
}
