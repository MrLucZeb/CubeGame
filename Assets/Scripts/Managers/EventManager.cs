using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Must be initialized first, before other managers
/// </summary>
public class EventManager : MonoBehaviour
{
    public UnityEvent<EntityMoveEvent> entityMoveEvent { get; private set; }
    public UnityEvent<EntityDeathEvent> entityDeathEvent { get; private set; }
    public UnityEvent<OverlapEvent> overlapEvent { get; private set; }
    public UnityEvent<TurnEvent> turnEvent { get; private set; }
    public UnityEvent<PlayerTurnEvent> playerTurnEvent { get; private set; }

    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        entityMoveEvent = new EntityMoveEvent();
        entityDeathEvent = new EntityDeathEvent();
        overlapEvent = new OverlapEvent();
        turnEvent = new TurnEvent();
        playerTurnEvent = new PlayerTurnEvent();
    }
}