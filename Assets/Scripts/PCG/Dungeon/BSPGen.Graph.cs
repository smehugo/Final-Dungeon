using System.Collections.Generic;
using UnityEngine;

public partial class BSPGen
{
    private void BuildEdgeList()
    {
        allEdges.Clear();
        //build the edge list for MST
        for (int i = 0; i < dungeonRooms.Count; i++)
        {
            for (int j = i + 1; j < dungeonRooms.Count; j++)
            {
                var a = dungeonRooms[i].bounds;
                var b = dungeonRooms[j].bounds;

                //check ovelap for straight corridors
                bool xGood = (a.xMax <= b.xMin || b.xMax <= a.xMin) && Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin) > 1;
                bool yGood = (a.yMax <= b.yMin || b.yMax <= a.yMin) && Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin) > 1; // holy check

                if (!xGood && !yGood)
                    continue;

                // add edge
                float length = Vector2Int.Distance(roomCenterPoints[i], roomCenterPoints[j]);
                allEdges.Add(new RoomEdge { a = i, b = j, dist = length });
            }
        }
        allEdges.Sort((a, b) => a.dist.CompareTo(b.dist));
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

    private void BuildMSTSecondPass()
    {
        //second pass to add extra connects
        var existing = new int[roomCenterPoints.Count];
        foreach (var edge in mstEdges)
        {
            existing[edge.a]++;
            existing[edge.b]++;
        }

        for (int roomId = 0; roomId < roomCenterPoints.Count; roomId++)
        {
            if (existing[roomId] >= 2)
            {
                continue;
            }
            foreach (var edge in allEdges)
            {
                // attached
                if (edge.a != roomId && edge.b != roomId)
                    continue;

                int otherId = edge.a == roomId ? edge.b : edge.a;

                // // already has 2
                // if (existing[otherId] >= 2)
                //     continue;

                // already in MST
                if (mstEdges.Contains(edge))
                    continue;

                mstEdges.Add(edge);
                existing[edge.a]++;
                existing[edge.b]++;
                break;
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
