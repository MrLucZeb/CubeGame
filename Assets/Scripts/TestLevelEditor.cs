using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GridEditorWindow : EditorWindow
{

    private bool[,] gridData;
    private int gridSizeX = 10;
    private int gridSizeY = 10;
    private int[,] grid;
    private int gridSize = 10; // number of tiles in the grid
    private int tileWidth = 20; // width of each tile in pixels
    private int tileHeight = 20; // height of each tile in pixels
    private bool isDragging = false;

    [MenuItem("Window/Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridEditorWindow>("Grid Editor");
    }

    private void OnEnable()
    {
        grid = new int[gridSize, gridSize];
    }

    List<Vector2Int> p = new List<Vector2Int>();

    private void OnGUI()
    {
        Event e = Event.current;

        // Handle mouse down event
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            isDragging = true;
            //GUI.FocusControl(null); // Unfocus control to allow dragging over buttons
        }

        // Handle mouse up event
        if (e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;
        }

        // Draw the grid tiles
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Rect rect = new Rect(j * tileWidth, i * tileHeight, tileWidth, tileHeight);
                GUI.Box(rect, grid[i, j].ToString());

                // Handle mouse clicks on the grid tiles
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    grid[i, j] = (grid[i, j] + 1) % 2;
                    Repaint();
                }

                if (rect.Contains(Event.current.mousePosition))
                {
                    if (isDragging && e.type == EventType.MouseDrag && e.button == 0)
                    {
                        if (!p.Contains(new Vector2Int(i, j)))
                        {
                            grid[i, j] = (grid[i, j] + 1) % 2;
                            Repaint();
                            p.Add(new Vector2Int(i, j));
                        }
                       
                    }
                }
            }
        }
    }

    private void OnGUeeI()
    {
        Event e = Event.current;

        // Handle mouse down event
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            isDragging = true;
            //GUI.FocusControl(null); // Unfocus control to allow dragging over buttons
        }

        // Handle mouse up event
        if (e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;
            p.Clear();
        }
        GUIContent buttonText = new GUIContent("some button");
        GUIStyle buttonStyle = GUIStyle.none;
        // Draw grid buttons
        GUILayout.BeginVertical();
        for (int y = 0; y < gridSizeY; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridSizeX; x++)
            {
                bool value = gridData[x, y];

                Rect rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
                if (rt.Contains(Event.current.mousePosition))
                {
                    if (isDragging && e.type == EventType.MouseDrag && e.button == 0)
                    {
                        rt.Set(400, 400, 0, 0);
                    }
                    GUI.Label(new Rect(0, 20, 200, 70), "PosX: " + rt.x + "\nPosY: " + rt.y +
                    "\nWidth: " + rt.width + "\nHeight: " + rt.height);
                }
                GUI.Button(rt, buttonText, buttonStyle);
                // Handle mouse drag event
                if (isDragging && e.type == EventType.MouseDrag && e.button == 0)
                {
                    // Rect buttonRect = GUILayoutUtility.GetRect(x * 30, y * 30);
                    // GUILayoutUtility.GetRect()
                    // if (buttonRect.Contains(e.mousePosition))
                    // {
                    //     gridData[x, y] = !value;
                    //}


                }

                //if (GUILayout.Toggle(value, "", GUILayout.Width(30), GUILayout.Height(30)))
                {
                    // gridData[x, y] = !value;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        // Save button
        if (GUILayout.Button("Save"))
        {
            SaveGridData();
        }
    }


    private void SaveGridData()
    {
        string path = EditorUtility.SaveFilePanel("Save Grid Data", Application.dataPath, "gridData", "txt");
        if (path.Length > 0)
        {
            string[] lines = new string[gridSizeY];
            for (int y = 0; y < gridSizeY; y++)
            {
                string line = "";
                for (int x = 0; x < gridSizeX; x++)
                {
                    line += gridData[x, y] ? "1" : "0";
                }
                lines[y] = line;
            }
            System.IO.File.WriteAllLines(path, lines);
        }
    }

    private void LoadGridData()
    {
        string path = EditorUtility.OpenFilePanel("Load Grid Data", Application.dataPath, "txt");
        if (path.Length > 0)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            gridSizeX = lines[0].Length;
            gridSizeY = lines.Length;
            gridData = new bool[gridSizeX, gridSizeY];
            for (int y = 0; y < gridSizeY; y++)
            {
                string line = lines[y];
                for (int x = 0; x < gridSizeX; x++)
                {
                    gridData[x, y] = line[x] == '1';
                }
            }
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnValidate()
    {
        if (gridSizeX < 1) gridSizeX = 1;
        if (gridSizeY < 1) gridSizeY = 1;
    }

    private void OnDestroy()
    {
        SaveGridData();
    }
}

