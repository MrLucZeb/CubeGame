using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType : int { NONE = 0, WALL = 1, PLAYER = 2, WALKER = 3, VOID = 4 }

[RequireComponent(typeof(MoveAnimation))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Entity : MonoBehaviour
{
    private MoveAnimation moveAnimation;
    public Vector2Int gridPosition { get; private set; }
    public bool hasMoved { get; protected set; } = false; // Whether entity has moved during its turn

    bool isInitialized = false;

    private void Awake()
    {
        moveAnimation = GetComponent<MoveAnimation>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("Entity (" + name + ") not intialized");
            return;
        }
           
    }

    // Called in order to initialize entity
    public void Init(Vector2Int position)
    {
        gridPosition = position;
        isInitialized = true;
    }

    //protected virtual void ExecuteMovement(Vector2Int direction)
    //{
    //    Vector2Int target = gridPosition + direction;

    //    //Debug.Log("previous: " + previousPosition + " target: " + gridPosition);

    //    EntityMoveEvent moveEvent = new EntityMoveEvent(this, target);
    //    Game.eventManager.entityMoveEvent.Invoke(moveEvent);
        
    //    StartCoroutine(MoveToTarget(direction));
    //    gridPosition = target;
    //}

    protected virtual void ExecuteMovement(Vector2Int target)
    {
        EntityMoveEvent moveEvent = new EntityMoveEvent(this, target);
        Game.eventManager.entityMoveEvent.Invoke(moveEvent);

        Vector3 targetWorldPosition = Game.Instance.GridToWorldPosition(target.x, target.y);

        StartCoroutine(MoveToTarget(targetWorldPosition));
        gridPosition = target;
    }

    // Executes movement in world space 
    IEnumerator MoveToTarget(Vector3 target)
    {
        Game game = Game.Instance;
        Vector3 start = transform.position;

        moveAnimation.Play();

        while (moveAnimation.IsPlaying()) {
            transform.position = Vector3.LerpUnclamped(start, target, moveAnimation.value);
            yield return null;
        }

        moveAnimation.Reset();
        
        yield break;
    }

    // Used for in game events
    public virtual void Kill() 
    {
        Game.eventManager.entityDeathEvent.Invoke(new EntityDeathEvent(this));
        Destruct();
    }

    // Only use directly for unloading
    public void Destruct()
    {
        Destroy(gameObject);
    }

    public abstract EntityType GetEntityType();
    public abstract bool OnTurn();
}
