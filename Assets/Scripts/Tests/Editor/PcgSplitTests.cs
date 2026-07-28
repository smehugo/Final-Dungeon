using UnityEngine;
using NUnit.Framework;

// macro BSP partition
public class PcgSplitTests : PcgTestBase
{
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Split_ProducesAtLeastOneLeaf(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        Assert.Greater(data.leaves.Count, 0);
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Leaves_AreInsideTheMap(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var leaf in data.leaves)
        {
            Assert.GreaterOrEqual(leaf.rect.xMin, 0);
            Assert.GreaterOrEqual(leaf.rect.yMin, 0);
            Assert.LessOrEqual(leaf.rect.xMax, DungeonGenConfig.MapWidth);
            Assert.LessOrEqual(leaf.rect.yMax, DungeonGenConfig.MapHeight);
        }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Leaves_DoNotOverlap(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        for (int i = 0; i < data.leaves.Count; i++)
            for (int j = i + 1; j < data.leaves.Count; j++)
                Assert.IsFalse(data.leaves[i].rect.Overlaps(data.leaves[j].rect),
                    $"leaves {i} and {j} overlap");
    }

    // partition doesnt duple any area
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Leaves_TileTheWholeMap(int seed)
    {
        var data = PcgTestGen.Generate(seed);

        int area = 0;
        foreach (var leaf in data.leaves)
        {
            area += leaf.rect.width * leaf.rect.height;
        }
        Assert.AreEqual(DungeonGenConfig.MapWidth * DungeonGenConfig.MapHeight, area);
    }

    // the cut range halves are at least MinLeafSize
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Leaves_AreAtLeastMinLeafSize(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var leaf in data.leaves)
        {
            Assert.GreaterOrEqual(leaf.rect.width, DungeonGenConfig.MinLeafSize);
            Assert.GreaterOrEqual(leaf.rect.height, DungeonGenConfig.MinLeafSize);
        }
    }

    // stops if the leaf counter = roomCount
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void LeafCount_DoesNotExceedRoomCount(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        Assert.LessOrEqual(data.leaves.Count, DungeonGenConfig.RoomCount);
    }
}
