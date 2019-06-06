using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MarketManager : MonoBehaviour
{

    public HorizontalLayoutGroup Menu;
    public MenuEntry MenuEntryPrefab;

    public MarketItem[] Items;

    private GameManager gm;
    private Cursor cursor;

    protected void Awake()
    {
        if (Menu == null)
            Debug.LogError($"{name} does not have a menu object assigned!");

        if (MenuEntryPrefab == null)
            Debug.LogError($"{name} does not have a menu entry prefab assigned!");

        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name} could not find game manager!");

        cursor = FindObjectOfType<Cursor>();

        if (cursor == null)
            Debug.LogError($"{name}: unable to find cursor!");
    }

    protected void Start()
    {
        foreach (MarketItem mi in Items)
        {
            MenuEntry me = Instantiate(MenuEntryPrefab, Menu.transform);
            me.SetPrice(mi.Price);
            me.SetSprite(mi.Icon);
            me.SetEnabled(mi.Enabled);
            me.SetAction(() => { MakePurchase(mi); });
        }
    }

    public void MakePurchase(MarketItem item)
    {
        if (gm.Parts < item.Price)
            return;

        cursor.BeginPlacing(item);
    }

    public int PriceByTile(TileBase tile)
    {
        foreach (MarketItem mi in Items)
            if (mi.Tile == tile)
                return mi.Price;

        return -1;
    }

}

[System.Serializable]
public class MarketItem
{

    public bool Enabled;
    public int Price;
    public Sprite Icon;
    public TileBase Tile;
    public bool Rotates;
    public TileBase NorthTile;
    public TileBase SouthTile;
    public TileBase EastTile;
    public TileBase WestTile;
    public bool HasSecond;
    public Sprite SecondIcon;
    public TileBase SecondTile;
    public bool MultiTile;
    public bool N;
    public bool S;
    public bool E;
    public bool W;

}