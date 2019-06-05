using UnityEngine;

public class SellButton : MonoBehaviour
{

    public float Threshold = 0.5f;

    GameManager gm;
    private Grid grid;
    private SpriteRenderer spriteRenderer;
    private bool available = false;

    protected void Awake()
    {
        // Find important components/objects
        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: no game manager found!");

        grid = FindObjectOfType<Grid>();

        if (grid == null)
            Debug.LogError($"{name}: could not find the Grid!");

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogError($"{name}: does not have a sprite renderer component!");

        // Set up initial values
        available = false;
    }

    protected void Update()
    {
        spriteRenderer.enabled = available;

        // Get mouse position in the world
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(mousePos, transform.position) < Threshold)
        {
            available = true;
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                // We were clicked
                if (hit.collider != null && hit.collider.transform == transform)
                {
                    gm.SellObject(grid.LocalToCell(transform.position));
                }
            }
        } else
        {
            available = false;
        }
    }

}
