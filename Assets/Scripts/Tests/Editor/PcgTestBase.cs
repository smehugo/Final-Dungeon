// helpers used by the special-room tests
public class PcgTestBase
{
    protected static DungeonRoom StartRoom(PcgTestGen.Data data)
    {
        foreach (var room in data.rooms)
        {
            if (room.isStartRoom)
            {
                return room;
            }
        }
        return null;
    }

    protected static DungeonRoom FinalRoom(PcgTestGen.Data data)
    {
        foreach (var room in data.rooms)
        {
            if (room.isFinalRoom)
            {
                return room;
            }
        }
        return null;
    }
}
