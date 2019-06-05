using UnityEngine;
using EventCallbacks;

public class Health : MonoBehaviour
{

    [HideInInspector]
    public int CurrentHealth = 15;
    public int MaxHealth = 15;
    public HealthBar HealthBarPrefab;
    private HealthBar healthBar;

    private Grid grid;
    private GameManager gm;

    protected void Awake()
    {
        grid = FindObjectOfType<Grid>();

        if (grid == null)
            Debug.LogError($"{name}: unable to find grid!");

        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: unable to find game manager!");

        if (HealthBarPrefab == null)
            Debug.LogError($"{name}: no health bar prefab assigned!");

        // Register listeners
        TileDamageEvent.RegisterListener(TakeDamage);
    }

    protected void Start()
    {
        int oldHealth = gm.GetHealth(grid.LocalToCell(transform.position));
        gm.AddRepairable(grid.LocalToCell(transform.position), this);
        CurrentHealth = (oldHealth == -1) ? MaxHealth : oldHealth;
        healthBar = Instantiate(HealthBarPrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity, transform);
        healthBar.percentage = (float)CurrentHealth / MaxHealth;
    }

    private void TakeDamage(TileDamageEvent e)
    {
        Vector2Int cellLoc = (Vector2Int)grid.LocalToCell(transform.position);
        if (e.CellLoc == cellLoc)
        {
            CurrentHealth -= e.DamageAmount;
            healthBar.percentage = (float)CurrentHealth / MaxHealth;
            gm.UpdateHealth((Vector3Int)cellLoc);

            if (CurrentHealth <= 0)
            {
                TileDestroyedEvent tileKilled = new TileDestroyedEvent
                {
                    Description = $"{name} tile was killed by {e.enemy.name}.",
                    CellLoc = cellLoc
                };
                tileKilled.TriggerEvent();

                Destroy(gameObject);
            }
        }
    }

    public void Repair()
    {
        CurrentHealth = MaxHealth;
        healthBar.percentage = (float)CurrentHealth / MaxHealth;
    }

    protected void OnDestroy()
    {
        TileDamageEvent.UnregisterListener(TakeDamage);
    }

}
