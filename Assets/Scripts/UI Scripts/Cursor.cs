using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using EventCallbacks;

public class Cursor : MonoBehaviour
{

    public Grid GridRef;
    public Tilemap ForbiddenMap;
    public TileBase ForbiddenTile;

    public Vector3Int MouseGridPos { get; private set; }
    public bool CursorDisabled { get; private set; }

    private GameManager gm;
    private SpriteRenderer spriteRenderer;
    private Sprite cursor;
    private DefenderMenuButton defenderMenuButton;

    private bool placing = false;
    private bool second = false;
    private CardinalDirection direction;
    private MarketItem toPlace;

    protected void Awake()
    {
        if (GridRef == null)
            Debug.LogError($"{name}: no grid object assigned!");

        if (ForbiddenMap == null)
            Debug.LogError($"{name}: no forbidden map assigned!");

        if (ForbiddenTile == null)
            Debug.LogError($"{name}: no forbidden tile assigned!");

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogError($"{name}: no graphic child game object!");

        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: no game manager found!");

        defenderMenuButton = FindObjectOfType<DefenderMenuButton>();

        if (defenderMenuButton == null)
            Debug.LogError($"{name}: no defender menu button found!");

        cursor = spriteRenderer.sprite;
        CursorDisabled = true;

        // Register listeners
        GameOverEvent.RegisterListener(GameOver);
        GameWonEvent.RegisterListener(GameWon);
        GameStartEvent.RegisterListener(GameStart);
        MenuEvent.RegisterListener(Menu);
    }

    private void GameStart(GameStartEvent e)
    {
        CursorDisabled = false;
    }

    private void GameWon(GameWonEvent e)
    {
        CursorDisabled = true;
    }

    private void GameOver(GameOverEvent e)
    {
        CursorDisabled = true;
    }

    private void Menu(MenuEvent e)
    {
        CursorDisabled = e.MenuOpen;
    }

    protected void OnDestroy()
    {
        GameOverEvent.UnregisterListener(GameOver);
        GameWonEvent.UnregisterListener(GameWon);
        GameStartEvent.UnregisterListener(GameStart);
        MenuEvent.UnregisterListener(Menu);
    }

    protected void Update()
    {
        if (CursorDisabled)
            return; 

        // Translate the mouse position to the position of a cell on the grid
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseGridPos = GridRef.LocalToCell(mousePos);
        transform.position = GridRef.CellToLocal(MouseGridPos);

        if (placing)
        {
            if (toPlace.Rotates && Input.GetKeyDown(GameManager.KeyBinds["Rotate"]))
            {
                direction = (CardinalDirection)(((int)direction + 1) % 4);
                switch(direction)
                {
                    case CardinalDirection.North:
                        spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        break;
                    case CardinalDirection.South:
                        spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                        break;
                    case CardinalDirection.East:
                        spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                        break;
                    case CardinalDirection.West:
                        spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        break;
                }
            }

            if (Input.GetMouseButtonUp(1))
                EndPlacing();
            else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                // We are placing in a valid location
                if (!ForbiddenMap.HasTile(MouseGridPos))
                {
                    gm.PlaceObject(toPlace, second, direction);
                    ForbiddenMap.SetTile(MouseGridPos, ForbiddenTile);
                    EndPlacing();
                    if (toPlace.HasSecond)
                    {
                        if (!second)
                        {
                            second = true;
                            BeginPlacing(toPlace, second);
                        } else
                            second = false;
                    } else if (gm.Parts >= toPlace.Price)
                        BeginPlacing(toPlace);
                }
            }
        }
    }

    public void BeginPlacing(MarketItem toPlace, bool second = false)
    {
        if (CursorDisabled)
            return;

        defenderMenuButton.SetMenu(false);

        ForbiddenMap.gameObject.SetActive(true);
        this.toPlace = toPlace;
        spriteRenderer.sprite = second ? toPlace.SecondIcon : toPlace.Icon;
        if (!toPlace.Rotates)
        {
            direction = CardinalDirection.North;
            spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        second = false;
        placing = true;
    }

    public void EndPlacing()
    {
        ForbiddenMap.gameObject.SetActive(false);
        spriteRenderer.sprite = cursor;
        placing = false;
    }

}
