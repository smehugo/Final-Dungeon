using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSPGen : MonoBehaviour
{
    [SerializeField] private int width = 48;
    [SerializeField] private int height = 48;
    [SerializeField] private int roomPadding;


    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [SerializeField] private int minRoomSize = 8;
    [SerializeField] private int maxDepth = 5;

    private BSPNode rootNode;

    [ContextMenu("Generate Dungeon")]

    private void GenerateDungeon()
    {
        rootNode = new BSPNode(new RectInt(0, 0, width, height));
        Split(rootNode, 0);

        //collect the final rects
        List<BSPNode> leaves = new List<BSPNode>();
        GetLeaves(rootNode, leaves);
        Debug.Log($"leaves: {leaves.Count}");
    }

    private void Split(BSPNode node, int currentDepth)
    {
        if (currentDepth >= maxDepth ||
            node.rect.width < minRoomSize * 2 ||
            node.rect.height < minRoomSize * 2)
            return;

        bool splitHorizontally = SplitDirection(node.rect, currentDepth);
        float splitRatio = Random.Range(0.3f, 0.7f); //gamba
        if (splitHorizontally)
        {
            int splitY = Mathf.RoundToInt(node.rect.yMin + node.rect.height * splitRatio);
            node.left = new BSPNode(new RectInt(node.rect.xMin, node.rect.yMin, node.rect.width, splitY - node.rect.yMin));
            node.right = new BSPNode(new RectInt(node.rect.xMin, splitY, node.rect.width, node.rect.yMax - splitY));
        }
        else
        {
            int splitX = Mathf.RoundToInt(node.rect.xMin + node.rect.width * splitRatio);
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

        Vector3 center = new Vector3(
            node.rect.x + node.rect.width * 0.5f,
            node.rect.y + node.rect.height * 0.5f,
            0f
        );

        Vector3 size = new Vector3(node.rect.width, node.rect.height, 0f);
        Gizmos.DrawWireCube(center, size);

        DrawNode(node.left);
        DrawNode(node.right);
    }
}
