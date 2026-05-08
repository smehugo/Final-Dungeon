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

    private int MinLeafSize => minRoomSize + 2 * roomPadding;

    private BSPNode rootNode;

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

            if (InW <= minRoomSize || InH <= minRoomSize)
                continue;

            int roomW = Random.Range(minRoomSize, InW + 1);
            int roomH = Random.Range(minRoomSize, InH + 1);
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
        if (rootNode == null)
            return;

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
