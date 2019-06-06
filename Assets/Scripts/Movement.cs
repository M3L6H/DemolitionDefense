using UnityEngine;
using EventCallbacks;

public class Movement : MonoBehaviour
{

    public float Speed = 1f;

    private GameManager gm;

    private Pathing pathing;
    // Keep track of what point we are at in our path
    private int pathIndex;
    private SpriteRenderer graphic;

    protected void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: no game manager found!");

        pathing = GetComponent<Pathing>();

        if (pathing == null)
            Debug.LogError($"{name}: No pathing component attached!");

        graphic = GetComponentInChildren<SpriteRenderer>();

        if (graphic == null)
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
        if (pathIndex >= 0 && pathIndex < pathing.Path.Count)
        {
            if (Arrived())
                pathIndex++;

            if (!gm.Paused && pathIndex < pathing.Path.Count)
            {
                Vector2 direction = pathing.Path[pathIndex] - (Vector2)transform.position;
                graphic.transform.right = direction;
                transform.position += (Vector3)direction.normalized * Speed * gm.GameSpeed * Time.deltaTime;
            }
        }
    }

    protected void OnDestroy()
    {
        TileUpdateEvent.UnregisterListener(TileUpdated);
    }

    // Recalculate our path when tiles have updated
    public void TileUpdated(TileUpdateEvent e)
    {
        CalculatePath();
    }

    // Called when something external moves us
    public void Moved()
    {
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
