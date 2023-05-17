using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Invoked when the next entity to make a turn is the player
/// </summary>
public class PlayerTurnEvent : UnityEvent<PlayerTurnEvent>
{
    Player player;

    public PlayerTurnEvent() { }

    public PlayerTurnEvent(Player player)
    {
        this.player = player;
    } 

    public Player GetPlayer()
    {
        return player;
    }
}
