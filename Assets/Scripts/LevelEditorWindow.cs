using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    Level level = null;
    string comment;
    int width = 0;
    int height = 0;
    int doubleMoveKillRequirement = 0;

    EntityType selection = EntityType.NONE;

    Event input;
    bool isDragging = false;
    List<Vector2Int> passedElements = new List<Vector2Int>(); // List of grid positions that have been changed since last drag

    Vector2Int playerTile; // Position of the tile that currently contains entity type PLAYER
    bool hasPlayerTile; // True if an entity type of PLAYER is present on the grid

    Dictionary<EntityType, Color> entityColorList = new Dictionary<EntityType, Color>();

    bool hasChanges = false;

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelEditorWindow));
    }

    void OnEnable()
    {
        foreach (EntityType value in System.Enum.GetValues(typeof(EntityType)))
        {
             entityColorList.Add(value, Color.gray);
        }

        Game game = Game.Instance;
        entityColorList[EntityType.WALL] = new Color(43/255, 43/255, 43/255);
        entityColorList[EntityType.PLAYER] = new Color(81/255, 172/255, 255/255);
        entityColorList[EntityType.WALKER] = new Color(255/255, 81/255, 81/255);
        entityColorList[EntityType.VOID] = Color.clear;
    }

    void OnDestroy()
    {
        Save();
    }

    void OnGUI()
    {
        RegisterInput();

        DrawLevelSelectField();

        if (level != null)
        {
            InsertIntField(ref width, "Current width: " + level.grid.width, GUILayout.MaxWidth(75));
            InsertIntField(ref height, "Current height: " + level.grid.height, GUILayout.MaxWidth(75));
            InsertIntField(ref doubleMoveKillRequirement, ": Current value " + level.doubleMoveKillRequirement, GUILayout.MaxWidth(75));
            InsertTextField(ref comment, "Level comment: " + level.comment, GUILayout.ExpandWidth(true), GUILayout.Width(225), GUILayout.MaxWidth(500));


            if (hasChanges || width != level.grid.width || height != level.grid.height || level.doubleMoveKillRequirement != doubleMoveKillRequirement || level.comment != comment)
            {
                if (Button("Save changes", GUILayout.Width(100)))
                {
                    Save();
                }
            }
            else
            {
                Button("", GUILayout.Width(0));
            }

            DrawSelectionMenu();

            DrawTiles();
        }
    }

    // Initializes editor variables when a new level is selected for editing
    void Init()
    {
        width = level.grid.width;
        height = level.grid.height;
        comment = level.comment;
        doubleMoveKillRequirement = level.doubleMoveKillRequirement;

        hasPlayerTile = false;
        level.grid.Foreach((x, y, type) => { 
            if (type == (int)EntityType.PLAYER)
            {
                playerTile = new Vector2Int(x, y);
                hasPlayerTile = true;

                return;
            }
        });
    }

    void Save()
    {
        if (level == null) return;

        level.grid.Resize(width, height);
        level.doubleMoveKillRequirement = doubleMoveKillRequirement;
        level.comment = comment;

        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();
        hasChanges = false;
    }

    private void DrawCenterIndicator(float heightOffset)
    {
        Handles.DrawLine(new Vector3(0, heightOffset), new Vector3(width, heightOffset + height));
    }

    private void RegisterInput()
    {
        input = Event.current;

        // Handle mouse down event
        if (input.type == EventType.MouseDown && (input.button == 0 || input.button == 1))
        {
            isDragging = true;
        }

        // Handle mouse up event
        if (input.type == EventType.MouseUp && (input.button == 0 || input.button == 1))
        {
            isDragging = false;
            passedElements.Clear();
        }
    }

    // Applies width and height changes to the level grid
    public void ApplyChanges()
    {
        level.grid.Resize(width, height);
        Save();
    }

    public void SetSelection(object entityType)
    {
        selection = (EntityType)entityType;
    }

    void DrawTiles()
    {
        float tileSize = 50;
        float tileOffset = 1; // Offset tiles from each other
        float heightOffset = EditorGUILayout.BeginVertical().y; // Prevent tiles from overlapping with other menu items

        // Draw center indicator 
        { 
        Rect centerX = new Rect((width - 1) * (tileSize + tileOffset) / 2.0f + tileSize / 4, heightOffset, tileSize / 2, height * (tileSize + tileOffset));
        Rect centerY = new Rect(0, (height - 1) * (tileSize + tileOffset) / 2.0f + heightOffset + tileSize / 4, width * (tileSize + tileOffset), tileSize / 2);
        GUI.Box(centerX, "");
        GUI.Box(centerY, "");
        }

        GUI.skin.box.fontSize = 10;
        GUI.skin.box.alignment = TextAnchor.MiddleCenter;

        level.grid.Foreach((x, y, type) =>
        {
            string entityTypeName = ((EntityType)type).ToString();
            Color entityTypeColor = entityColorList[(EntityType)type];

            Rect rect = new Rect(x * (tileSize + tileOffset), (height - 1 - y) * (tileSize + tileOffset) + heightOffset, tileSize, tileSize);
            GUI.color = entityTypeColor;
            GUI.Box(rect, entityTypeName);

            // Check if mouse is hovering over rect
            if (rect.Contains(Event.current.mousePosition))
            {
                bool rightMouse = Event.current.button == 1;

                // Continue if mouse is being clicked or dragged
                if (Event.current.type == EventType.MouseDown || isDragging && input.type == EventType.MouseDrag && selection != EntityType.PLAYER)
                {
                    Vector2Int tile = new Vector2Int(x, y);
                    int entityType = (int)selection;

                    if (rightMouse)
                        entityType = (int)EntityType.NONE;

                    if (entityType == (int)EntityType.PLAYER)
                    {
                        // If a player tile is present, replace it with entity type NONE
                        if (hasPlayerTile)
                            level.grid.Set(playerTile.x, playerTile.y, (int)EntityType.NONE);

                        playerTile = tile;  // Store the position of the new tile containg PLAYER 
                        hasPlayerTile = true;
                    }
                    else if (hasPlayerTile && tile == playerTile) // Check if tile containing enity type PLAYER is being removed
                    {
                        hasPlayerTile = false;
                    }

                    level.grid.Set(x, y, entityType);
                    hasChanges = true;
                    Repaint();
                }
            }
        });
    }

    private void DrawLevelSelectField()
    {
        Level previous = level;
        level = (Level)EditorGUILayout.ObjectField(level, typeof(Level), false, GUILayout.MaxWidth(125));

        if (level != previous && level != null)
        {
            Init();
        }
    }

    private void DrawSelectionMenu()
    {
        if (GUILayout.Button(selection.ToString(), GUILayout.MaxWidth(75)))
        {
            GenericMenu dropdown = new GenericMenu();

            foreach (EntityType entityType in System.Enum.GetValues(typeof(EntityType)))
            {
                dropdown.AddItem(new GUIContent(entityType.ToString()), false, SetSelection, entityType);
            }

            dropdown.ShowAsContext();
        }
    }

    private void InsertIntField(ref int value, string text, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal();

        value = EditorGUILayout.IntField(value, options);
        EditorGUILayout.HelpBox(text, MessageType.None);

        EditorGUILayout.EndHorizontal();
    }

    private void InsertTextField(ref string value, string text, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal();

        value = EditorGUILayout.TextField(value, options);
        EditorGUILayout.HelpBox(text, MessageType.None);

        EditorGUILayout.EndHorizontal();
    }

    private bool Button(string text, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal();

        bool result = GUILayout.Button(text, options);

        EditorGUILayout.EndHorizontal();

        return result;
    }
}
