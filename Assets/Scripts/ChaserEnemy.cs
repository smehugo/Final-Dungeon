using System.Collections.Generic;
using UnityEngine;

// bfs on mapdata not tilemap
public class ChaserEnemy : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float repathTime = 0.25f;
    [SerializeField] private float damageCD = 0.5f;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sr;

    private DungeonMapData mapData;
    private DungeonRoom spawnRoom;
    private Transform player;
    private Rigidbody2D rb;
    private List<Vector2Int> currentPath;
    private int currentPathId;
    private bool roomHasPlayer;
    private float nextRepathTime;
    private float nextDmgTime;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(DungeonMapData mapData, DungeonRoom room)
    {
        this.mapData = mapData;
        spawnRoom = room;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (mapData == null || isDead)
        {
            return;
        }

        Vector2Int playerTile = mapData.GetTileFromWorldPos(player.position);

        // outside spawn room idle
        if (!spawnRoom.bounds.Contains(playerTile))
        {
            roomHasPlayer = false;
            return;
        }

        roomHasPlayer = true;

        if (Time.time < nextRepathTime)
        {
            return;
        }

        nextRepathTime = Time.time + repathTime;
        Repath();
    }

    private void FixedUpdate()
    {
        if (!roomHasPlayer || currentPath == null || currentPathId >= currentPath.Count || isDead)
        {
            return;
        }
        // keep rb kinematic!!
        Vector3 targetPos = mapData.GetWorldPosFromTile(currentPath[currentPathId]);
        Vector2 dir = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
        rb.MovePosition(dir);

        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            currentPathId++;
        }

        Vector2 delta = dir - rb.position;
        animator.SetBool("isMoving", delta.sqrMagnitude > 0.001f);
        if (Mathf.Abs(delta.x) > 0.01f) sr.flipX = delta.x < 0;
    }

    private void Repath()
    {
        Vector2Int startTile = mapData.GetTileFromWorldPos(transform.position);
        Vector2Int goalTile = mapData.GetTileFromWorldPos(player.position);

        currentPath = AStar.FindPath(mapData, startTile, goalTile, spawnRoom.bounds);
        if (currentPath == null)
        {
            return;
        }

        // skip the i stand on
        if (currentPath.Count > 1)
            currentPathId = 1;
        else
            currentPathId = 0;

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!roomHasPlayer || !collision.CompareTag("Player"))
            return;

        if (Time.time < nextDmgTime)
            return;

        nextDmgTime = Time.time + damageCD;

        if (collision.TryGetComponent(out PlayerHealth health))
            health.TakeDamage(damage);
        animator.SetTrigger("attack");
    }

    public void SetDead()
    {
        isDead = true;
        animator.SetTrigger("die");
    }
}
