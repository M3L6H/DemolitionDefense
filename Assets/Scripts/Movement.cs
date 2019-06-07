using UnityEngine;
using UnityEngine.Tilemaps;
using EventCallbacks;

public class Movement : MonoBehaviour
{

    public float Speed = 1f;
    public bool Heavy = false;
    public bool Flying = false;
    public TileBase Hole;

    private GameManager gm;

    private Pathing pathing;
    // Keep track of what point we are at in our path
    private int pathIndex;
    private SpriteRenderer[] graphics;
    private bool beingMoved = false;
    private float movedSpeed;
    private Vector3 movedTargetLoc;

    protected void Awake()
    {
        if (Hole == null)
            Debug.LogError($"{name}: no hole assigned!");

        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: no game manager found!");

        pathing = GetComponent<Pathing>();

        if (pathing == null)
            Debug.LogError($"{name}: No pathing component attached!");

        graphics = GetComponentsInChildren<SpriteRenderer>();

        if (graphics[0] == null)
            Debug.LogError($"{name}: missing child graphic object!");

        // Register listeners
        TileUpdateEvent.RegisterListener(TileUpdated);
    }

    protected void Start()
    {
        CalculatePath();
    }

    protected void Update()
    {
        if (beingMoved)
        {
            transform.position += (movedTargetLoc - transform.position).normalized * movedSpeed * Time.deltaTime * gm.GameSpeed;
            if (Vector2.Distance(transform.position, movedTargetLoc) < 0.05f * gm.GameSpeed)
            {
                beingMoved = false;
                Moved();
            }
        } else
        {
            if (pathIndex >= 0 && pathIndex < pathing.Path.Count)
            {
                if (Arrived())
                    pathIndex++;

                if (!gm.Paused && pathIndex < pathing.Path.Count)
                {
                    Vector2 direction = pathing.Path[pathIndex] - (Vector2)transform.position;
                    foreach (SpriteRenderer graphic in graphics)
                        graphic.transform.right = direction;
                    transform.position += (Vector3)direction.normalized * Speed * gm.GameSpeed * Time.deltaTime;
                }
            }
        }
    }

    protected void OnDestroy()
    {
        TileUpdateEvent.UnregisterListener(TileUpdated);
    }

    public void MoveTo(Vector3 targetLoc, float speed)
    {
        beingMoved = true;
        movedTargetLoc = targetLoc;
        movedSpeed = speed;
    }

    // Recalculate our path when tiles have updated
    public void TileUpdated(TileUpdateEvent e)
    {
        CalculatePath();
    }

    // Called when something external moves us
    public void Moved()
    {
        Vector3 roundedLoc = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0);
        if (!Flying && pathing.LevelMap.GetTile(pathing.LevelMap.LocalToCell(roundedLoc)) == Hole)
            Destroy(gameObject);
        CalculatePath();
    }

    // Calculate our path and reset our path index accordingly
    private void CalculatePath()
    {
        pathing.CalculatePath();
        pathIndex = 0;
    }

    // Determines if we are currently arrived at a node in our path
    private bool Arrived()
    {
        return Mathf.Abs(transform.position.x - pathing.Path[pathIndex].x) < 0.05f * gm.GameSpeed &&
               Mathf.Abs(transform.position.y - pathing.Path[pathIndex].y) < 0.05f * gm.GameSpeed;
    }

}
