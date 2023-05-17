using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Invoked when two entities overlap on the level grid
public class OverlapEvent : UnityEvent<OverlapEvent>
{
    private Entity overlapper;
    private Entity subject;

    public OverlapEvent() { }

    // overlapper = entity which move caused the overlap
    // subject = entity that did not move
    public OverlapEvent(Entity overlapper, Entity subject)
    {
        this.overlapper = overlapper;
        this.subject = subject;
    }

    public Entity GetOverlapper()
    {
        return overlapper;
    }

    public Entity GetSubject()
    {
        return subject;
    }
}
