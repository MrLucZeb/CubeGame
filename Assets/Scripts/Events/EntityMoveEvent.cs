using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Invoked when entity will be movoing
public class EntityMoveEvent : UnityEvent<EntityMoveEvent>
{
    private Entity entity;
    private Vector2Int position;
    private Vector2Int targetPosition;

    public EntityMoveEvent() { }

    public EntityMoveEvent(Entity entity, Vector2Int targetPosition) {
        this.entity = entity;
        this.targetPosition = targetPosition;
        position = entity.gridPosition;
    }

    public Entity GetEntity()
    {
        return entity;
    }

    // Current position of the entity before movement
    public Vector2Int GetPosition()
    {
        return position;
    }

    // Position of the entity after movement
    public Vector2Int GetTargetPosition()
    {
        return targetPosition;
    }
}
