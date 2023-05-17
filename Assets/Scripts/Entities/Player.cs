using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public bool canDoubleMove = false;
    private Vector2Int nextMove = Vector2Int.zero;
    private bool hasNextMove = false;
    public int doubleMoveMeter = 0; // Counts the amount of kills until meter is full, acquiring a double move

    public override EntityType GetEntityType()
    {
        return EntityType.PLAYER;
    }

    public void SetNextMove(Vector2Int position)
    {
        // TODO Check if move is possible
        nextMove = position;
        hasNextMove = true;
    }

    /// <summary>
    /// Returns a list of positions this player can move to
    /// </summary>
    public List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        AddAvailableMoves(moves, Vector2Int.up);
        AddAvailableMoves(moves, Vector2Int.down);
        AddAvailableMoves(moves, Vector2Int.right);
        AddAvailableMoves(moves, Vector2Int.left);

        return moves;
    }

    private void AddAvailableMoves(List<Vector2Int> moves, Vector2Int direction)
    {
        Vector2Int move = gridPosition + direction;
        if (!IsBlocked(direction))
        {
            moves.Add(move);

            Vector2Int doubleMove = gridPosition + direction * 2;
            if (canDoubleMove && !IsBlocked(direction * 2))
            {
                moves.Add(doubleMove);
            }
        } else if (!IsBlocked(direction, true, true))
        {
            moves.Add(move);
        }
    }

    public override bool OnTurn()
    {
        if (hasNextMove)
        {
            if (Vector2Int.Distance(gridPosition, nextMove) > 1)
                canDoubleMove = false; // Take away double move when it has been used

            ExecuteMovement(nextMove);
            hasMoved = true;
            hasNextMove = false;

            return true;
        }

        return false;
    }

    private bool IsBlocked(Vector2Int move, bool allowMoveOnWall = false, bool allowMoveOnEntity = false)
    {
        Vector2Int position = gridPosition + move;
        Grid2D<int> grid = Game.levelManager.level.grid;

        if (!grid.IsPositionValid(position.x, position.y)) return true;
        if (grid.Get(position) == (int)EntityType.WALL)
        { 
            if (move.magnitude == 1 && allowMoveOnWall)
            {
                // Allow the player to move to a wall tile if the tile next to it is empty, causing player to move the wall.
                Vector2Int position2 = gridPosition + move * 2;
                if (!grid.IsPositionValid(position2.x, position2.y)) return true;
                if (grid.Get(position2) != (int)EntityType.NONE) return true;
            } else
            {
                return true;
            }
        }
        if (grid.Get(position) != (int)EntityType.NONE)
        {
            if (!allowMoveOnEntity || grid.Get(position) == (int)EntityType.VOID) return true;
        }

        return false;
    }

    //public override bool OnTurn()
    //{
    //    int horizontal = (Input.GetKeyDown(KeyCode.D)) ? 1 : (Input.GetKeyDown(KeyCode.A)) ? -1 : 0;
    //    int vertical = (Input.GetKeyDown(KeyCode.W)) ? 1 : (Input.GetKeyDown(KeyCode.S)) ? -1 : 0;

    //    Dictionary<Vector2Int, EntityType> surrounding = Game.levelManager.GetSurrounding(gridPosition, EntityType.WALL);

    //    if (doubleMove)
    //    {
    //        horizontal *= 2;
    //        vertical *= 2;
    //    }

    //    if (Input.GetKeyDown(KeyCode.LeftShift) && surrounding.Count == 0)
    //    {
    //        doubleMove = !doubleMove;
    //        Debug.Log("Double Move: " + doubleMove);
    //    }

    //    if (surrounding.Count > 0)
    //        doubleMove = false;

    //    if (horizontal != 0 ^ vertical != 0)
    //    {
    //        Vector2Int direction = new Vector2Int(horizontal, vertical);

    //        if (IsBlocked(direction)) return false; // Prevent player from moving in a wall or off the grid


    //        if (surrounding.Count == 0 || surrounding.ContainsKey(gridPosition + direction)) // Force player to surrounding tiles with entities if any are present
    //        {
    //            ExecuteMovement(direction);
    //            doubleMove = false;

    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //private bool IsBlocked(Vector2Int direction)
    //{
    //    Grid2D grid = Game.levelManager.level.grid;
    //    Vector2Int position = gridPosition + direction;

    //    if (!grid.IsPositionValid(position.x, position.y)) return true;
    //    if (grid.Get(position) == (int) EntityType.WALL) return true;
    //    if (doubleMove)
    //    {
    //        position = gridPosition + new Vector2Int(direction.x/2, direction.y/2);
    //        if (grid.Get(position) == (int)EntityType.WALL) return true;
    //    }

    //    return false;
    //}
}
