using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Entity
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override EntityType GetEntityType()
    {
        return EntityType.WALKER;
    }

    public override bool OnTurn()
    {
        Vector2Int target = Game.levelManager.player.gridPosition;
        List<Vector2Int> path = PathFinder.FindPath(gridPosition, target, EntityType.WALL, EntityType.VOID, EntityType.WALL);
        Game.Instance.DrawDebugPath(path);

        if (path.Count > 1)
        {
            ExecuteMovement(path[1]);
            hasMoved = true;
        } else
        {
            hasMoved = false;
        }

        return true;
    }
}
