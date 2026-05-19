using System.Collections.Generic;
using UnityEngine;

public partial class BSPGen
{
    private void Split(BSPNode node, int currentDepth)
    {
        if (currentDepth >= maxDepth || currentLeafCount >= roomCount)
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

        currentLeafCount++;
        Split(node.left, currentDepth + 1);
        Split(node.right, currentDepth + 1);
    }

    private void GetLeaves(BSPNode node, List<BSPNode> leaves)
    {
        if (node == null) return;

        if (node.left == null && node.right == null)
        {
            leaves.Add(node);
            return;
        }

        GetLeaves(node.left, leaves);
        GetLeaves(node.right, leaves);
    }

    private bool SplitDirection(RectInt rect, int depth)
    {
        float ratio = (float)rect.width / rect.height;

        if (ratio > 1.35f) return false;
        if (ratio < 0.75f) return true;

        return depth % 2 == 0;
    }
}
