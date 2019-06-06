using UnityEngine;
using EventCallbacks;

public class TrapIn : MonoBehaviour
{

    public TrapCollider Collider;

    public bool Paired;

    private TrapOut trapOut;
    private TrapCollider trapCollider;
    private GameManager gm;

    protected void Awake()
    {
        if (Collider == null || Collider.GetComponent<BoxCollider2D>() == null)
            Debug.LogError($"{name}: No collider assigned!");

        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: Could not find game manager!");

        Paired = false;

        // Set up listeners
        TileSoldEvent.RegisterListener(Sell);
    }

    protected void Start()
    {
        trapCollider = Instantiate(Collider);
        trapCollider.transform.position = transform.position;

        TrapOut[] traps = FindObjectsOfType<TrapOut>();

        foreach (TrapOut trap in traps)
        {
            if (!trap.Paired)
            {
                trapOut = trap;
                Paired = true;
                trapOut.SetIn(this);
                break;
            }
        }
    }

    public void SetOut(TrapOut trapOut)
    {
        this.trapOut = trapOut;
        trapCollider.MyTrapOut = trapOut;
        Paired = true;
    }

    public void Sell(TileSoldEvent e)
    {
        if (e.CellLoc == (Vector2Int)gm.LevelMap.LocalToCell(transform.position))
            gm.DestroyTile(gm.LevelMap.LocalToCell(trapOut.transform.position));
    }

    protected void OnDestroy()
    {
        trapOut.Paired = false;
        TileSoldEvent.UnregisterListener(Sell);
    }

}
