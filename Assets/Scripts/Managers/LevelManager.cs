using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Level level { get; private set; } = null;
    public List<Entity> entities { get; private set; } = new List<Entity>();
    public Player player { get; private set; } = null;

    void Awake()
    {
        AddListeners();
    }

    void AddListeners()
    {
        EventManager eventManager = Game.eventManager;
        eventManager.entityMoveEvent.AddListener(OnEntityMove);
        eventManager.entityDeathEvent.AddListener(OnEntityDeath);
    }

    public void Load(Level level)
    {
        this.level = level.Clone();

        entities.ForEach(entity => entity.Destruct());
        entities.Clear();

        Game game = Game.Instance;
        level.grid.Foreach((x, y, type) =>
        {
            EntityType entityType = (EntityType) type;
            GameObject entity = game.SpawnEntity(entityType, x, y);

            if (entity != null)
            {
                if (entityType == EntityType.PLAYER)
                {
                    player = entity.GetComponent<Player>();
                    entities.Insert(0, player); // Make sure player is first in the list
                } else
                {
                    entities.Add(entity.GetComponent<Entity>());
                }
            }
        });
    }

    public Entity GetEntityAtPosition(Vector2Int position)
    {
        if (level.grid.Get(position) == (int)EntityType.NONE) return null;

        Entity result = null;

        entities.ForEach(entity =>
        {
            if (entity.gridPosition == position)
            {
                result = entity;
                return; // TODO check if this works
            }
        });
        
        return result;
    }

    // Get non diagonal surrounding enties
    public Dictionary<Vector2Int, EntityType> GetSurrounding(Vector2Int position, params EntityType[] exclude)
    {
        Dictionary<Vector2Int, EntityType> result = new Dictionary<Vector2Int, EntityType>();

        for (int x = position.x - 1; x < position.x + 2; x++)
        {
            for (int y = position.y - 1; y < position.y + 2; y++)
            {
                if (x == position.x && y == position.y) continue; // Prevent diagnals and same as position
                if (x != position.x && y != position.y) continue;
                if (!level.grid.IsPositionValid(x, y))  continue;

                EntityType entity = (EntityType) level.grid.Get(x, y);

                if (!exclude.Contains(entity) && entity != EntityType.NONE)
                {
                    result.Add(new Vector2Int(x, y), entity);
                }
            }
        }

        return result;
    }

    public void OnEntityMove(EntityMoveEvent e)
    {
        Vector2Int current = e.GetPosition();
        Vector2Int target = e.GetTargetPosition();

        if (level.grid.Get(target) != (int)EntityType.NONE)
        {
            OverlapEvent overlapEvent = new OverlapEvent(e.GetEntity(), GetEntityAtPosition(target)); //TODO call movvement event before move has been executed
            Game.eventManager.overlapEvent.Invoke(overlapEvent);
        }

        // Update position of entity on the level grid
        level.grid.Set(current, (int)EntityType.NONE); // TODO replace 0 with macro
        level.grid.Set(target, (int)e.GetEntity().GetEntityType()); // TODO replace 0 with entity id
    }

    void OnEntityDeath(EntityDeathEvent e)
    {
        entities.Remove(e.GetEntity());
    }
}
