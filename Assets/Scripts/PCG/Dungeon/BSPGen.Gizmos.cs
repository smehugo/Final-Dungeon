using Unity.VisualScripting;
using UnityEngine;

public partial class BSPGen
{
    // private void OnDrawGizmos()
    // {
    //     if (rootNode == null)
    //         return;

    //     DrawNode(rootNode);

    //     Gizmos.color = Color.red;
    //     foreach (var corridor in corridors)
    //     {
    //         DrawRect(corridor, Color.red);
    //     }

    //     // doors
    //     if (dungeonRooms != null)
    //     {
    //         foreach (var room in dungeonRooms)
    //         {
    //             foreach (var door in room.doors)
    //             {
    //                 DrawRect(new RectInt(door.position.x, door.position.y, 1, 1), Color.yellow);
    //             }
    //         }
    //     }

    //     // reserved blokcs
    //     if (dungeonRooms != null)
    //     {
    //         foreach (var room in dungeonRooms)
    //         {
    //             foreach (var tile in room.reservedTiles)
    //             {
    //                 DrawRect(new RectInt(tile.x, tile.y, 1, 1), Color.magenta);
    //             }
    //         }
    //     }

    //     // interior
    //     if (dungeonRooms != null)
    //     {
    //         foreach (var room in dungeonRooms)
    //         {
    //             foreach (var zone in room.zones)
    //             {
    //                 DrawRect(zone.bounds, Color.cyan);
    //             }

    //             foreach (var wall in room.interiorWalls)
    //             {
    //                 foreach (var tile in wall.tiles)
    //                 {
    //                     DrawRect(new RectInt(tile.x, tile.y, 1, 1), Color.white);
    //                 }
    //             }
    //         }
    //     }
    // }

    private void DrawNode(BSPNode node)
    {
        if (node == null) return;

        // DrawRect(node.rect, Color.white);

        DrawNode(node.left);
        DrawNode(node.right);

        if (node.hasRoom)
        {
            DrawRect(node.roomRect, Color.green);
        }
    }

    private void DrawRect(RectInt rect, Color color)
    {
        Gizmos.color = color;

        Vector3 center = new Vector3(
            rect.x + rect.width * 0.5f,
            rect.y + rect.height * 0.5f,
            0f
        );

        Vector3 size = new Vector3(rect.width, rect.height, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}
