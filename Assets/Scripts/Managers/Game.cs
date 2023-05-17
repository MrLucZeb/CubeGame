using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    public static EventManager eventManager { get; private set; } // Must be initialized before other managers
    public static LevelManager levelManager { get; private set; }
    public static TurnManager turnManager { get; private set; }
    public static TileManager tileManager { get; private set; }

    public GameObject WALL;
    public GameObject PLAYER;
    public GameObject WALKER;

    public GameObject TILE;
    public GameObject VOID_TILE;

    [SerializeField] private List<Level> levels = new List<Level>();
    private int currentLevel = 0;

    public List<Vector2Int> debugPath = new List<Vector2Int>();

    public int tileSize { get; private set; } = 1; // Size of a single tile in world space

    [SerializeField] private bool debugDrawGrid = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        InitializeManagers();
        AddListeners();
    }

    void Start()
    {
        LoadLevel(currentLevel);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadLevel(currentLevel);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentLevel--;
            if (currentLevel < 0) currentLevel = levels.Count - 1;

            LoadLevel(currentLevel);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentLevel++;
            if (currentLevel >= levels.Count) currentLevel = 0;

            LoadLevel(currentLevel);
        }
    }

    void LoadLevel(int level)
    {
        levelManager.Load(levels[level]);
        tileManager.Load();
        turnManager.Load();
    }

    public void AddListeners()
    {
        eventManager.entityMoveEvent.AddListener(OnEntityMove);
        eventManager.entityDeathEvent.AddListener(OnEntityDeath);
        eventManager.turnEvent.AddListener(OnTurn);
        eventManager.overlapEvent.AddListener(OnOverlap);
    }

    void InitializeManagers()
    {
        if ((eventManager = gameObject.GetComponent<EventManager>()) == null)
            eventManager = gameObject.AddComponent<EventManager>();

        if ((levelManager = gameObject.GetComponent<LevelManager>()) == null)
            levelManager = gameObject.AddComponent<LevelManager>();

        if ((turnManager = gameObject.GetComponent<TurnManager>()) == null)
            turnManager = gameObject.AddComponent<TurnManager>();

        if ((tileManager = gameObject.GetComponent<TileManager>()) == null)
            tileManager = gameObject.AddComponent<TileManager>();
    }

    public void Pause(bool pause = true)
    {
        turnManager.Pause(pause);
    }

    public GameObject SpawnEntity(EntityType entity, int x, int y)
    {
        GameObject objectBase;

        switch (entity)
        {
            default:
                return null;
            case EntityType.WALL:
                objectBase = WALL;
                break;
            case EntityType.PLAYER:
                objectBase = PLAYER;
                break;
            case EntityType.WALKER:
                objectBase = WALKER;
                break;
        }

        GameObject gameObject = Instantiate(objectBase, GridToWorldPosition(x, y), Quaternion.identity, transform);
        gameObject.GetComponent<Entity>().Init(new Vector2Int(x, y));

        return gameObject;
    }

    private Vector3 GetBottomLeft()
    {
        float x = transform.position.x - (float)levelManager.level.grid.width / 2 + (float)tileSize / 2;
        float y = transform.position.y - (float)levelManager.level.grid.height / 2 + (float)tileSize / 2;

        return new Vector3(x, y, transform.position.z);
    }

    public Vector3 GridToWorldPosition(int x, int y)
    {
        Vector3 origin = GetBottomLeft();
        return new Vector3(origin.x + x * tileSize, origin.y + y * tileSize, origin.z);
    }

    void OnEntityMove(EntityMoveEvent e)
    {
        Dictionary<Vector2Int, EntityType> surrounding = levelManager.GetSurrounding(e.GetTargetPosition(), e.GetEntity().GetEntityType(), EntityType.WALL, EntityType.VOID);

        if (surrounding.Count > 0)
        {
            turnManager.SetTurn(e.GetEntity());
        }
    }

    void OnEntityDeath(EntityDeathEvent e)
    {
        if (e.GetEntity().GetEntityType() == EntityType.PLAYER)
        {
            Pause();
        }
    }

    void OnTurn(TurnEvent e)
    {
        
    }

    void OnOverlap(OverlapEvent e)
    {
        if (e.GetSubject().GetEntityType() == EntityType.WALL)
        {
            // Move the wall away from the player (Player pushing wall)
            Vector2Int playerMoveDirection = e.GetSubject().gridPosition - e.GetOverlapper().gridPosition;
            ((Wall)e.GetSubject()).Move(playerMoveDirection);
        }
        else
        {
            if (e.GetOverlapper().GetEntityType() == EntityType.PLAYER)
            {
                Player player = (Player) e.GetOverlapper();
                player.doubleMoveMeter++;

                if (player.doubleMoveMeter >= levelManager.level.doubleMoveKillRequirement)
                {
                    player.canDoubleMove = true;
                    player.doubleMoveMeter = 0;
                }
            }

            e.GetSubject().Kill();
        }
    }

    public void DrawDebugPath(List<Vector2Int> path)
    {
        debugPath = path;
    }

    void OnDrawGizmos()
    {
        // Draw grid gizmos
        if (levelManager != null && levelManager.level != null)
        {
            if (debugDrawGrid)
            {
                Gizmos.color = Color.white;

                levelManager.level.grid.Foreach((x, y, type) =>
                {
                    Gizmos.DrawSphere(GridToWorldPosition(x, y), .1f);
                });
            }
        }

        // Draw path gizmos
        if (debugPath.Count > 0)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < debugPath.Count - 2; i++)
            {
                Gizmos.DrawLine(GridToWorldPosition(debugPath[i].x, debugPath[i].y), GridToWorldPosition(debugPath[i + 1].x, debugPath[i + 1].y));
            }
        }
    }
}


//List<Vector2Int> path = new List<Vector2Int>();
//Vector2Int start;
//Vector2Int finish;
//void Update()
//{
//    if (Input.GetKey(KeyCode.E))
//    {
//        int x1 = UnityEngine.Random.Range(0, levelManager.level.grid.width - 1);
//        int y1 = UnityEngine.Random.Range(0, levelManager.level.grid.height - 1);
//        int x2 = UnityEngine.Random.Range(0, levelManager.level.grid.width - 1);
//        int y2 = UnityEngine.Random.Range(0, levelManager.level.grid.height - 1);

//        start = new Vector2Int(x1, y1);
//        finish = new Vector2Int(x2, y2);

//        if (start != finish)
//            path = PathFinder.FindPath(start, finish, EntityType.WALL);
//    }
//}

//Gizmos.color = Color.red;
//Gizmos.DrawSphere(GridToWorldPosition(finish.x, finish.y), .15f);

//Gizmos.color = Color.green;
//Gizmos.DrawSphere(GridToWorldPosition(start.x, start.y), .15f);

//Vector3 a = GetBottomLeft();
