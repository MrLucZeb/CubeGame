using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Entity
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
        return EntityType.WALL;
    }

    public override bool OnTurn()
    {
        return true;
    }

    public void Move(Vector2Int direction)
    {
        ExecuteMovement(gridPosition + direction);
    }
}
