using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityDeathEvent : UnityEvent<EntityDeathEvent>
{
    Entity entity;

    public EntityDeathEvent() { }

    public EntityDeathEvent(Entity entity)
    {
        this.entity = entity;
    }

    public Entity GetEntity()
    {
        return entity;
    }
}
