using NUnit.Framework;
using UnityEngine;

// candidate edges, MST, second pass
public class PcgGraphTests : PcgTestBase
{
    // edges
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EdgeList_IsNotEmpty(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        if (data.rooms.Count < 2) Assert.Inconclusive("not enough rooms");
        Assert.Greater(data.allEdges.Count, 0);
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Edges_ReferenceValidRoomIndices(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var edge in data.allEdges)
        {
            Assert.GreaterOrEqual(edge.a, 0);
            Assert.GreaterOrEqual(edge.b, 0);
            Assert.Less(edge.a, data.rooms.Count);
            Assert.Less(edge.b, data.rooms.Count);
            Assert.AreNotEqual(edge.a, edge.b, "edge joins a room to itself");
        }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Edges_AreSortedByDistance(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        for (int i = 1; i < data.allEdges.Count; i++)
            Assert.LessOrEqual(data.allEdges[i - 1].dist, data.allEdges[i].dist);
    }

    // straight
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Edges_OnlyConnectRoomsThatCouldTakeAStraightCorridor(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var edge in data.allEdges)
        {
            var a = data.rooms[edge.a].bounds;
            var b = data.rooms[edge.b].bounds;

            bool xGood = (a.xMax <= b.xMin || b.xMax <= a.xMin)
                && Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin) > 1;
            bool yGood = (a.yMax <= b.yMin || b.yMax <= a.yMin)
                && Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin) > 1;

            Assert.IsTrue(xGood || yGood, $"edge {edge.a}-{edge.b} has no straight corridor option");
        }
    }

    // mst subset
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void MstEdges_AreASubsetOfTheEdgeList(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var mst in data.mstEdges)
        {
            bool found = false;
            foreach (var candidate in data.allEdges)
                if (SameEdge(mst, candidate)) { found = true; break; }

            Assert.IsTrue(found, $"mst edge {mst.a}-{mst.b} is not in the candidate list");
        }
    }

    // mst <= n-1
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Mst_HasAtMostRoomsMinusOneEdges(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        if (data.rooms.Count < 2) Assert.Inconclusive("not enough rooms");
        Assert.LessOrEqual(data.mstBaseEdges.Count, data.rooms.Count - 1);
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Mst_HasNoCycles(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        Assert.IsFalse(HasCycle(data.rooms.Count, data.mstBaseEdges), "the mst contains a cycle");
    }

    // mst connected
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Mst_FormsOneConnectedComponent(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        if (data.rooms.Count < 2) Assert.Inconclusive("not enough rooms");

        int components = CountComponents(data.rooms.Count, data.mstBaseEdges);
        Assert.AreEqual(1, components, $"the mst is a forest of {components} components");
    }

    // keep og mst on second
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void SecondPass_KeepsEveryMstEdge(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var baseEdge in data.mstBaseEdges)
        {
            bool found = false;
            foreach (var edge in data.mstEdges)
                if (SameEdge(baseEdge, edge)) { found = true; break; }

            Assert.IsTrue(found, $"second pass dropped mst edge {baseEdge.a}-{baseEdge.b}");
        }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryRoom_AppearsInAtLeastOneMstEdge(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        if (data.rooms.Count < 2) Assert.Inconclusive("not enough rooms");

        var used = new bool[data.rooms.Count];
        foreach (var edge in data.mstEdges)
        {
            used[edge.a] = true;
            used[edge.b] = true;
        }

        for (int i = 0; i < used.Length; i++)
            Assert.IsTrue(used[i], $"room {i} is in no mst edge");
    }
}
