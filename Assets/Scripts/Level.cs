using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 1)]
[System.Serializable]
public class Level : ScriptableObject
{
    [SerializeField] public Grid2D<int> grid;
    [SerializeField] public int doubleMoveKillRequirement = 2; // Amount of kills required to get a double move
    [SerializeField] public string comment;

    private void OnEnable()
    {
        if (grid == null)
        {
            grid = new Grid2D<int>(16, 9);
            doubleMoveKillRequirement = 2;
        }
    }

    public Level Clone()
    {
        Level clone = CreateInstance<Level>();
        clone.grid = grid.Clone();
        clone.doubleMoveKillRequirement = doubleMoveKillRequirement;

        return clone;
    }
}
