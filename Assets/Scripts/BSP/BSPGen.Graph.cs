using System.Collections.Generic;
using UnityEngine;

public partial class BSPGen
{
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

    private void BuildMST()
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
