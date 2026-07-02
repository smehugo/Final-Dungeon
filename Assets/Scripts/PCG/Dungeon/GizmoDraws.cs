using System.Collections.Generic;
using UnityEngine;

public class GizmoDraws : MonoBehaviour
{
    [SerializeField] private BSPGen generator;
    [Header("Draw:")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private bool drawBspLeaves = true;
    [SerializeField] private bool drawRooms = true;
    [SerializeField] private bool drawRoomCenters = true;
    [SerializeField] private bool drawAllGraphEdges = false;
    [SerializeField] private bool drawMstGraph = true;
    [SerializeField] private bool drawCorridors = true;
    [SerializeField] private bool drawDoors = true;
    [SerializeField] private bool drawReservedTiles = false;
    [SerializeField] private bool drawInteriorZones = true;
    [SerializeField] private bool drawInteriorWalls = true;
    [SerializeField] private bool drawFinalFloorTiles = false;
    [SerializeField] private bool drawFinalCorridorTiles = false;
    [SerializeField] private bool drawFinalWallTiles = false;
    [SerializeField] private bool drawBlockedTiles = false;
    [SerializeField] private bool drawDebugPath = true;

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        if (drawBspLeaves && generator.DebugRootNode != null)
            BspLeaves(generator.DebugRootNode);

        if (drawFinalFloorTiles)
            Tiles(generator.DebugFinalFloorTiles, Color.darkGreen);

        if (drawFinalCorridorTiles)
            Tiles(generator.DebugFinalCorridorTiles, Color.red);

        if (drawFinalWallTiles)
            Tiles(generator.DebugFinalWallTiles, Color.white);

        if (drawBlockedTiles)
            Tiles(generator.DebugBlockedTiles, Color.magenta);

        if (drawRooms)
            Rooms(generator.DebugDungeonRooms);

        if (drawRoomCenters)
            Centers(generator.DebugDungeonRooms);

        if (drawAllGraphEdges)
            Edges(generator.DebugAllEdges, generator.DebugRoomCenters, Color.cyan);

        if (drawMstGraph)
            Edges(generator.DebugMstEdges, generator.DebugRoomCenters, Color.yellow);

        if (drawCorridors)
            Corridors(generator.DebugCorridors);

        RoomDetails(generator.DebugDungeonRooms);

        if (drawDebugPath)
            DebugPath(generator.DebugPath);
    }

    private void BspLeaves(BSPNode node)
    {
        if (node == null)
            return;

        if (node.left == null && node.right == null)
        {
            DrawRect(node.rect, Color.gray);
            return;
        }

        BspLeaves(node.left);
        BspLeaves(node.right);
    }

    private void Rooms(List<DungeonRoom> rooms)
    {
        if (rooms == null)
            return;

        foreach (var room in rooms)
        {
            Color color = Color.green;

            if (room.isStartRoom)
            {
                color = Color.blue;
            }
            else if (room.isFinalRoom)
            {
                color = Color.red;
            }
            else if (room.hasArtifact)
            {
                color = Color.limeGreen;
            }

            DrawRect(room.bounds, color);
        }
    }

    private void Centers(List<DungeonRoom> rooms)
    {
        if (rooms == null)
            return;

        foreach (var room in rooms)
        {
            Color color = Color.green;

            if (room.isStartRoom)
            {
                color = Color.blue;
            }
            else if (room.isFinalRoom)
            {
                color = Color.red;
            }
            else if (room.hasArtifact)
            {
                color = Color.hotPink;
            }

            DrawSphere(room.center, color, 0.25f);
        }
    }

    private void Edges(List<RoomEdge> edges, List<Vector2Int> centers, Color color)
    {
        if (edges == null || centers == null)
            return;

        Gizmos.color = color;

        foreach (var edge in edges)
        {
            if (edge.a < 0 || edge.a >= centers.Count)
                continue;

            if (edge.b < 0 || edge.b >= centers.Count)
                continue;

            Vector3 a = TileCenter(centers[edge.a]);
            Vector3 b = TileCenter(centers[edge.b]);

            Gizmos.DrawLine(a, b);
        }
    }

    private void Corridors(List<RectInt> corridors)
    {
        if (corridors == null)
            return;

        foreach (var corridor in corridors)
        {
            DrawFilledRect(corridor, Color.red);
            DrawRect(corridor, Color.red);
        }
    }

    private void RoomDetails(List<DungeonRoom> rooms)
    {
        if (rooms == null)
            return;

        foreach (var room in rooms)
        {
            if (drawDoors)
            {
                foreach (var door in room.doors)
                {
                    DrawTile(door.position, Color.yellow);
                }
            }

            if (drawReservedTiles)
            {
                foreach (var tile in room.reservedTiles)
                {
                    DrawTile(tile, Color.magenta);
                }
            }

            if (drawInteriorZones)
            {
                foreach (var zone in room.zones)
                {
                    DrawRect(zone.bounds, GetZoneColor(zone.type));
                }
            }

            if (drawInteriorWalls)
            {
                foreach (var wall in room.interiorWalls)
                {
                    foreach (var tile in wall.tiles)
                    {
                        DrawTile(tile, Color.white);
                    }
                }
            }
        }
    }

    private void Tiles(HashSet<Vector2Int> tiles, Color color)
    {
        if (tiles == null)
            return;

        foreach (var tile in tiles)
        {
            DrawTile(tile, color);
        }
    }

    private void DebugPath(List<Vector2Int> path)
    {
        if (path.Count == 0)
            return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count - 1; i++)
            Gizmos.DrawLine(TileCenter(path[i]), TileCenter(path[i + 1]));

        foreach (var tile in path)
            DrawTile(tile, new Color(0f, 1f, 1f, 0.35f));

        DrawSphere(path[0], Color.green, 0.2f);
        DrawSphere(path[path.Count - 1], Color.red, 0.2f);
    }

    private void DrawTile(Vector2Int tile, Color color)
    {
        Gizmos.color = color;

        Vector3 center = TileCenter(tile);
        Vector3 size = new Vector3(0.9f, 0.9f, 0f);

        Gizmos.DrawCube(center, size);
    }

    private void DrawFilledRect(RectInt rect, Color color)
    {
        Gizmos.color = color;

        Vector3 center = new Vector3(
            rect.x + rect.width * 0.5f,
            rect.y + rect.height * 0.5f,
            0f
        );

        Vector3 size = new Vector3(rect.width, rect.height, 0f);
        Gizmos.DrawCube(center, size);
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

    private void DrawSphere(Vector2Int tile, Color color, float radius)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(TileCenter(tile), radius);
    }

    private Vector3 TileCenter(Vector2Int tile)
    {
        return new Vector3(tile.x + 0.5f, tile.y + 0.5f, 0f);
    }

    private Color GetZoneColor(ZoneType type)
    {
        switch (type)
        {
            case ZoneType.Enemy:
                return Color.red;

            case ZoneType.Treasure:
                return Color.yellow;

            case ZoneType.Artifact:
                return Color.magenta;

            case ZoneType.Decoration:
                return Color.cyan;

            case ZoneType.Open:
                return Color.green;

            case ZoneType.Empty:
                return Color.gray;

            default:
                return Color.white;
        }
    }
}