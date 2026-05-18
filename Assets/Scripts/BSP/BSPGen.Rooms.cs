using System.Collections.Generic;
using UnityEngine;

public partial class BSPGen
{
    public int roomsCreated = 0;

    private void MakeRooms(List<BSPNode> leaves)
    {
        foreach (var leaf in leaves)
        {
            int InX = leaf.rect.xMin + roomPadding;
            int InY = leaf.rect.yMin + roomPadding;
            int InW = leaf.rect.width - 2 * roomPadding;
            int InH = leaf.rect.height - 2 * roomPadding;

            if (InW < minRoomSize || InH < minRoomSize)
                continue;

            int roomW = Mathf.RoundToInt(Random.Range(roomFillMin, roomFillMax) * InW);
            int roomH = Mathf.RoundToInt(Random.Range(roomFillMin, roomFillMax) * InH);
            int maxRoomX = InX + InW - roomW;
            int maxRoomY = InY + InH - roomH;
            int roomX = Random.Range(InX, maxRoomX + 1);
            int roomY = Random.Range(InY, maxRoomY + 1);

            leaf.roomRect = new RectInt(roomX, roomY, roomW, roomH);
            leaf.hasRoom = true;
            roomsCreated++;
            if (!leaf.hasRoom)
                Debug.Log("no room");
        }
    }

    private void GetRoomCenters()
    {
        List<BSPNode> roomNodes = new List<BSPNode>();
        rootNode.GetRooms(roomNodes);
        Debug.Log($"rooms: {roomNodes.Count}");

        roomCenterPoints.Clear();
        dungeonRooms.Clear();

        for (int i = 0; i < roomNodes.Count; i++)
        {
            var room = roomNodes[i];
            // Debug.Log($"room {i} rect: {room.roomRect}");

            Vector2Int center = new Vector2Int(
                room.roomRect.x + room.roomRect.width / 2,
                room.roomRect.y + room.roomRect.height / 2
            );
            roomCenterPoints.Add(center);
            dungeonRooms.Add(new DungeonRoom
            {
                id = i,
                bounds = room.roomRect,
                center = center
            });
        }
    }
}