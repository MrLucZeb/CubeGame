using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    Grid2D<Tile> tiles = null;
    List<Tile> moveOptionTiles = new List<Tile>();

    Tile selection = null;

    // Start is called before the first frame update
    void Awake()
    {
        Game.eventManager.playerTurnEvent.AddListener(OnPlayerTurn);
        Game.eventManager.turnEvent.AddListener(OnTurn);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        Tile tile;
        if (hit && (tile = hit.collider.gameObject.GetComponent<Tile>()) != null)
        {
            if (Input.GetMouseButtonDown(0))
                OnTileClicked(tile); // TODO refactor this

            if (selection != tile)
            {
                if (selection != null)
                {
                    selection.isHovered = false;
                    selection.UpdateState();
                }

                selection = tile;
                selection.isHovered = true;
                selection.UpdateState();
            }
        }
        else if (selection != null)
        {
            selection.isHovered = false;
            selection.UpdateState();
            selection = null;
        }
    }

    public void Load()
    {
        if (tiles != null)
        {
            moveOptionTiles.Clear();
            selection = null;
            tiles.Foreach((x, y, tile) =>
            {
                Destroy(tile.gameObject);
            });
        }

        Game game = Game.Instance;
        Grid2D<int> level = Game.levelManager.level.grid;

        tiles = new Grid2D<Tile>(level.width, level.height);

        Game.levelManager.level.grid.Foreach((x, y, type) =>
        {
            Vector3 position = game.GridToWorldPosition(x, y);
            position.z = -1;

            bool isVoid = type == (int) EntityType.VOID;
            GameObject gameObject = Instantiate((isVoid) ? game.VOID_TILE : game.TILE, position, Quaternion.identity, game.transform);

            Tile tile = gameObject.GetComponent<Tile>();
            tile.Init(new Vector2Int(x, y), isVoid);

            tiles.Set(x, y, tile);
        });
    }

    void OnPlayerTurn(PlayerTurnEvent e)
    {
        // Set moveable tiles for new player position
        e.GetPlayer().GetAvailableMoves().ForEach(position =>
        {
            Tile tile = tiles.Get(position);
            tile.isMoveOption = true;
            tile.UpdateState();

            moveOptionTiles.Add(tile);
        });
    }

    void OnTurn(TurnEvent e)
    {
        // Revert previous moveable tiles
        moveOptionTiles.ForEach(tile =>
        {
            tile.isMoveOption = false;
            tile.UpdateState();
        });
        moveOptionTiles.Clear();
    }

    void OnTileClicked(Tile tile)
    {
        if (tile.isMoveOption)
        {
            Game.levelManager.player.SetNextMove(tile.gridPosition);
        }   
    }
}
