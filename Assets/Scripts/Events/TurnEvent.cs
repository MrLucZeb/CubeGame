using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Invoked after a turn has been executed
/// </summary>
public class TurnEvent : UnityEvent<TurnEvent>
{
    private Entity entity;
    private int turn;

    bool repeatTurn = false;

    public TurnEvent() { }

    public TurnEvent(Entity entity, int turn)
    {
        this.entity = entity;
        this.turn = turn;
    }

    public Entity GetEntity() 
    {
        return entity;
    }

    public int GetTurn()
    {
        return turn;
    }

    public bool GetRepeatTurn()
    {
        return repeatTurn;
    }

    public void RepeatTurn(bool repeat)
    {
        repeatTurn = repeat;
    }
}
 