using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid2D<T>
{
    [field: SerializeField] public int width { get; private set; }
    [field: SerializeField] public int height { get; private set; }
    [field: SerializeField] public T[] data { get; private set; }

    public Grid2D(int width, int height)
    {
        Init(width, height);
    }

    private void Init(int width, int height)
    {
        this.width = width;
        this.height = height;
        data = new T[width * height];
    }

    public Grid2D<T> Clone()
    {
        Grid2D<T> clone = new Grid2D<T>(width, height);
        data.CopyTo(clone.data, 0);

        return clone;
    }

    /// Resets the value of all positions on the grid
    /// <param name="value"> Value to reset data to (0 by default)</param>
    public void Reset(T value)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Set(x, y, value);
            }
        }
    }

    public void Resize(int width, int height)
    {
        if (this.width == width && this.height == height) return;

        Grid2D<T> old = Clone();

        Init(width, height);

        int xMax = (this.width <= old.width) ? this.width : old.width;
        int yMax = (this.height <= old.height) ? this.height : old.height;
        for (int x = 0; x < xMax; x++)
        {
            for (int y = 0; y < yMax; y++)
            {
                Set(x, y, old.Get(x, y));
            }
        }
    }

    public bool IsPositionValid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    /// <summary> Returns the value of a position on the grid
    public T Get(int x, int y)
    {
        if (!IsPositionValid(x, y))
        {
            Debug.LogError("Tried to access grid data at invalid position (" + x + ", " + y + ")");
        }

        return data[x + y * width];
    }

    public T Get(Vector2Int position)
    {
        return Get(position.x, position.y);
    }

    /// <summary> Sets the value of a position on the grid
    public void Set(int x, int y, T value)
    {
        data[x + y * width] = value;
    }

    public void Set(Vector2Int position, T value)
    {
        Set(position.x, position.y, value);
    }

    public void Foreach(System.Action<int, int, T> action)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                action(x, y, Get(x, y));
            }
        }

    }
}
