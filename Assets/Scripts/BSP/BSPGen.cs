using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSPGen : MonoBehaviour
{
    [SerializeField] private int MapWidth = 48;
    [SerializeField] private int MapHeight = 48;
    [SerializeField] private int roomPadding;


    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [SerializeField] private int minRoomSize = 8;
    [SerializeField] private int maxDepth = 5;
    [SerializeField] private float roomFillMin = 0.5f;
    [SerializeField] private float roomFillMax = 0.9f;

    private int MinLeafSize => minRoomSize + 2 * roomPadding;

    private BSPNode rootNode;
    private List<RoomEdge> allEdges = new List<RoomEdge>();
    private List<Vector2Int> roomCenterPoints = new List<Vector2Int>();
    private List<RoomEdge> mstEdges = new List<RoomEdge>();
    private int[] MSTparent;
    private int[] MSTrank;
    private List<RectInt> corridors = new List<RectInt>();

    [ContextMenu("Generate Dungeon")]

    private void GenerateDungeon()
    {
        rootNode = new BSPNode(new RectInt(0, 0, MapWidth, MapHeight));
        Split(rootNode, 0);

        //collect the final rects
        List<BSPNode> leaves = new List<BSPNode>();
        GetLeaves(rootNode, leaves);
        Debug.Log($"leaves: {leaves.Count}");
        MakeRooms(leaves);

        //MST
        GetRoomCenters();
        BuildEdgeList();
        BuildMST();
        BuildCorridors();

    }

    private void BuildCorridors()
    {
        corridors.Clear();
        foreach (var edge in mstEdges)
        {
            Vector2Int a = roomCenterPoints[edge.a];
            Vector2Int b = roomCenterPoints[edge.b];

            if (Random.value < 0.5f)
            {
                //hor then vert
                corridors.Add(new RectInt(Mathf.Min(a.x, b.x), a.y, Mathf.Abs(a.x - b.x) + 1, 1));
                corridors.Add(new RectInt(b.x, Mathf.Min(a.y, b.y), 1, Mathf.Abs(a.y - b.y) + 1));
            }
            else
            {
                //vert then hor
                corridors.Add(new RectInt(a.x, Mathf.Min(a.y, b.y), 1, Mathf.Abs(a.y - b.y) + 1));
                corridors.Add(new RectInt(Mathf.Min(a.x, b.x), b.y, Mathf.Abs(a.x - b.x) + 1, 1));
            }
        }
    }

    private void BuildMST(int RoomCount = 0)
    {
        MSTparent = new int[roomCenterPoints.Count];
        MSTrank = new int[roomCenterPoints.Count];
        for (int i = 0; i < roomCenterPoints.Count; i++)
        {
            MSTparent[i] = i;
            MSTrank[i] = 0;
        }

        mstEdges.Clear();
        foreach (var edge in allEdges)
        {
            if (Find(edge.a) != Find(edge.b) && mstEdges.Count < roomCenterPoints.Count - 1)
            {
                mstEdges.Add(edge);
                Union(edge.a, edge.b);
            }
        }
        // Debug.Log($"mst edges: {mstEdges.Count}");
        // foreach (var edge in mstEdges)        {
        //     Debug.Log($"mst edge: {edge.a} to {edge.b} dist: {edge.dist}");
        // }
    }

    private void BuildEdgeList()
    {
        //build the edge list for MST
        allEdges.Clear();
        for (int i = 0; i < roomCenterPoints.Count; i++)
        {
            for (int j = i + 1; j < roomCenterPoints.Count; j++)
            {
                float dist = Vector2Int.Distance(roomCenterPoints[i], roomCenterPoints[j]);
                allEdges.Add(new RoomEdge(i, j, dist));
            }
        }

        allEdges.Sort((a, b) => a.dist.CompareTo(b.dist));
        // Debug.Log($"edges: {allEdges.Count}");
        // foreach (var edge in allEdges)
        // {
        //     Debug.Log($"edge: {edge.a} to {edge.b} dist: {edge.dist}");
        // }
    }

    private void GetRoomCenters()
    {
        //collect the room centers for corridor generation
        List<BSPNode> roomNodes = new List<BSPNode>();
        roomNodes.Clear();
        rootNode.GetRooms(roomNodes);
        Debug.Log($"rooms: {roomNodes.Count}");

        roomCenterPoints.Clear();
        foreach (var room in roomNodes)
        {
            Vector2Int center = new Vector2Int(
                room.roomRect.x + room.roomRect.width / 2,
                room.roomRect.y + room.roomRect.height / 2
            );
            roomCenterPoints.Add(center);
        }
    }

    private void Split(BSPNode node, int currentDepth)
    {
        if (currentDepth >= maxDepth)
            return;

        bool CanSplitHor = node.rect.height >= MinLeafSize * 2;
        bool CanSplitVert = node.rect.width >= MinLeafSize * 2;

        if (!CanSplitHor && !CanSplitVert)
            return;

        bool splitHorizontally = CanSplitHor && (!CanSplitVert || SplitDirection(node.rect, currentDepth));
        if (splitHorizontally)
        {
            int minCutY = node.rect.yMin + MinLeafSize;
            int maxCutY = node.rect.yMax - MinLeafSize;
            int splitY = Random.Range(minCutY, maxCutY + 1);
            node.left = new BSPNode(new RectInt(node.rect.xMin, node.rect.yMin, node.rect.width, splitY - node.rect.yMin));
            node.right = new BSPNode(new RectInt(node.rect.xMin, splitY, node.rect.width, node.rect.yMax - splitY));
        }
        else
        {
            int minCutX = node.rect.xMin + MinLeafSize;
            int maxCutX = node.rect.xMax - MinLeafSize;
            int splitX = Random.Range(minCutX, maxCutX + 1);
            node.left = new BSPNode(new RectInt(node.rect.xMin, node.rect.yMin, splitX - node.rect.xMin, node.rect.height));
            node.right = new BSPNode(new RectInt(splitX, node.rect.yMin, node.rect.xMax - splitX, node.rect.height));
        }
        Split(node.left, currentDepth + 1);
        Split(node.right, currentDepth + 1);

    }

    private void GetLeaves(BSPNode node, List<BSPNode> leaves)
    {
        if (node == null)return;

        if (node.left == null && node.right == null)
        {
            leaves.Add(node);
            return;
        }

        GetLeaves(node.left, leaves);
        GetLeaves(node.right, leaves);
    }

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

            leaf.roomRect = new RectInt(
                roomX,
                roomY,
                roomW,
                roomH
            );
            leaf.hasRoom = true;
            if (!leaf.hasRoom)
                Debug.Log("no room");
        }

    }





    // so we dont have long and skinny rooms, alternating on ratio
    private bool SplitDirection(RectInt rect, int depth)
    {
        float ratio = (float)rect.width / rect.height;

        if (ratio > 1.35f) return false;
        if (ratio < 0.75f) return true;

        return depth % 2 == 0;
    }

    private void OnDrawGizmos()
    {
        DrawNode(rootNode);
    }

    private void DrawNode(BSPNode node)
    {
        if (node == null) return;

        DrawRect(node.rect, Color.white);

        DrawNode(node.left);
        DrawNode(node.right);

        if (node.hasRoom)
        {
            DrawRect(node.roomRect, Color.green);
        }

        foreach (var corridor in corridors)
        {
            DrawRect(corridor, Color.red);
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

        Vector3 size = new(rect.width, rect.height, 0f);
        Gizmos.DrawWireCube(center, size);
    }

    private int Find(int i)
    {
        if (MSTparent[i] != i)
            MSTparent[i] = Find(MSTparent[i]);
        return MSTparent[i];
    }

    private void Union(int a, int b)
    {
        int rootA = Find(a);
        int rootB = Find(b);

        if (rootA == rootB)
            return;

        if (MSTrank[rootA] < MSTrank[rootB])
            MSTparent[rootA] = rootB;
        else if (MSTrank[rootB] < MSTrank[rootA])
            MSTparent[rootB] = rootA;
        else
        {
            MSTparent[rootB] = rootA;
            MSTrank[rootA]++;
        }
    }
}