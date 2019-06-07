using UnityEngine;
using EventCallbacks;
using System.Collections;

[System.Serializable]
public class Enemy : MonoBehaviour
{

    [Range(0, 1000)]
    public int BaseDamage = 10;

    [Range(0, 100)]
    public int DefenseDamage = 5;

    [Range(0, 1000)]
    public int Value = 20;

    [Range(0.5f, 5)]
    public float AttackSpeed = 2f;

    public Vector2Int CellLoc { get; private set; }

    private GameManager gm;
    private Grid grid;
    private Pathing pathing;

    protected void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: could not find game manager!");

        grid = FindObjectOfType<Grid>();

        if (grid == null)
            Debug.LogError($"{name}: could not find a grid!");

        pathing = GetComponent<Pathing>();

        if (pathing == null)
            Debug.LogError($"{name}: does not have a pathing component attached!");
    }

    protected void Start()
    {
        CellLoc = (Vector2Int)grid.LocalToCell(new Vector3(Mathf.Round(transform.position.x),
                                                           Mathf.Round(transform.position.y),
                                                           0f));
        StartCoroutine("DamageTiles");
    }

    private IEnumerator DamageTiles()
    {
        while(true)
        {
            if (!gm.Paused)
            {
                // Damage the four adjacent tiles
                TileDamageEvent[] damageEvents =
                {
                    new TileDamageEvent
                    {
                        Description = $"{name} damaged the tiles around {CellLoc} dealing {DefenseDamage} damage.",
                        CellLoc = CellLoc + Vector2Int.left,
                        DamageAmount = DefenseDamage,
                        enemy = this
                    },
                    new TileDamageEvent
                    {
                        Description = $"{name} damaged the tiles around {CellLoc} dealing {DefenseDamage} damage.",
                        CellLoc = CellLoc + Vector2Int.right,
                        DamageAmount = DefenseDamage,
                        enemy = this
                    },
                    new TileDamageEvent
                    {
                        Description = $"{name} damaged the tiles around {CellLoc} dealing {DefenseDamage} damage.",
                        CellLoc = CellLoc + Vector2Int.up,
                        DamageAmount = DefenseDamage,
                        enemy = this
                    },
                    new TileDamageEvent
                    {
                        Description = $"{name} damaged the tiles around {CellLoc} dealing {DefenseDamage} damage.",
                        CellLoc = CellLoc + Vector2Int.down,
                        DamageAmount = DefenseDamage,
                        enemy = this
                    }
                };

                foreach (TileDamageEvent e in damageEvents)
                    e.TriggerEvent();
            }

            // Wait for alloted amount of time
            float timeElapsed = 0f;

            while (timeElapsed <= AttackSpeed * 0.5f / gm.GameSpeed)
            {
                if (!gm.Paused)
                    timeElapsed += GameManager.TimeStep;
                yield return new WaitForSeconds(GameManager.TimeStep);
            }
        }
    }

    protected void Update()
    {
        Vector2Int currCellLoc = (Vector2Int)grid.LocalToCell(new Vector3(Mathf.Round(transform.position.x), 
                                                                          Mathf.Round(transform.position.y), 
                                                                          0f));
        if (currCellLoc != CellLoc)
            CellLoc = currCellLoc;

        foreach(Goal g in pathing.Goals)
        {
            // We have invaded the base
            if (Vector3.Distance(transform.position, g.transform.position) < 0.1f)
            {
                // Create base damage event and broadcast it
                BaseDamageEvent e = new BaseDamageEvent
                {
                    Description = $"{name} invaded the base dealing {BaseDamage} damage.",
                    DamageAmount = BaseDamage
                };

                e.TriggerEvent(); 

                Destroy(gameObject);
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Enemies should not recycle each other
        if (collision.gameObject.GetComponentInParent<Recycler>() != null)
            Recycle();
    }

    public void Recycle()
    {
        EnemyRecycledEvent e = new EnemyRecycledEvent
        {
            Description = $"{name} got recycled for {Value} parts.",
            Value = Value
        };

        e.TriggerEvent();
        Destroy(gameObject);
    }

}
