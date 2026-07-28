using UnityEngine;
using UnityEngine.Tilemaps;

// blocks final room until all artifacts collected
public class FinalRoomSeal : MonoBehaviour
{
    [SerializeField] private Tilemap sealTilemap;
    [SerializeField] private TileBase sealTile;

    private bool isSealed;

    public void BuildSeal(DungeonMapData mapData)
    {
        sealTilemap.ClearAllTiles();
        isSealed = false;

        DungeonRoom finalRoom = mapData.rooms.Find(r => r.isFinalRoom);

        for (int x = finalRoom.bounds.xMin; x < finalRoom.bounds.xMax; x++)
        {
            for (int y = finalRoom.bounds.yMin; y < finalRoom.bounds.yMax; y++)
            {
                // prop tiles dimmed too
                if (mapData.IsWalkable(new Vector2Int(x, y)))
                    sealTilemap.SetTile(new Vector3Int(x, y, 0), sealTile);
            }
        }

        isSealed = true;
    }

    private void Update()
    {
        if (!isSealed) return;

        if (ArtifactManager.AllArtifactsCollected())
        {
            sealTilemap.ClearAllTiles();
            isSealed = false;
            Debug.Log("final room good to goo!");
        }
    }
}