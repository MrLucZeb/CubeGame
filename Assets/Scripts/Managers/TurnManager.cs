using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    int previousTurnIndex = -1;
    int turnIndex = 0;
    int turnOverride = -1;
    // TODO use level manager's entitie list instead
    List<EntityType> excludedEntities = new List<EntityType>() { EntityType.WALL }; // Entities that should be excluded from the queue
    private float turnCooldown = 0.2f;

    public void Load()
    {
        previousTurnIndex = -1;
        turnIndex = 0;
        turnOverride = -1;

        StopCoroutine("TurnRequestCourotine");
        StartCoroutine("TurnRequestCourotine");
    }

    IEnumerator TurnRequestCourotine()
    {
        LevelManager levelManager = Game.levelManager;

        for (;;)
        {
            if (turnIndex >= levelManager.entities.Count) { turnIndex = 0; }
            if (turnOverride != -1)
            {
                turnIndex = turnOverride;
                turnOverride = -1;

                if (levelManager.entities[turnIndex].GetEntityType() == EntityType.PLAYER)
                {
                    PlayerTurnEvent playerTurnEvent = new PlayerTurnEvent(levelManager.player);
                    Game.eventManager.playerTurnEvent.Invoke(playerTurnEvent);
                }
            }

                Entity entity = levelManager.entities[turnIndex];

            if (excludedEntities.Contains(entity.GetEntityType())) {
                turnIndex++;
                continue;
            }

            if (previousTurnIndex != turnIndex)
            {
                if (levelManager.entities[turnIndex].GetEntityType() == EntityType.PLAYER)
                {
                    PlayerTurnEvent playerTurnEvent = new PlayerTurnEvent(levelManager.player);
                    Game.eventManager.playerTurnEvent.Invoke(playerTurnEvent);
                }

                previousTurnIndex = turnIndex;
            }

            if (entity.OnTurn())
            {
                TurnEvent turnEvent = new TurnEvent(entity, turnIndex);
                Game.eventManager.turnEvent.Invoke(turnEvent);

                turnIndex++;

                // Do not apply turn cooldown if entity has not moved during its turn
                if (entity.hasMoved)
                    yield return new WaitForSeconds(turnCooldown);
            }

            yield return null;
        }
    }

    public void Pause(bool pause = true)
    {
        if (pause)
            StopCoroutine("TurnRequestCourotine");
        else
            StartCoroutine("TurnRequestCourotine");
    }

    public void OverrideTurn(int index)
    {
        turnOverride = index;
    }

    public void SetTurn(Entity entity)
    {
        OverrideTurn(Game.levelManager.entities.IndexOf(entity));
    }
}