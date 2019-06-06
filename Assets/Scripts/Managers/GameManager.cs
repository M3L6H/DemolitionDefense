using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using EventCallbacks;

public class GameManager : MonoBehaviour
{

    public int BaseHealth;
    public int Parts;

    public float Return = 0.5f;

    public TileBase Dirt;
    public Tilemap LevelMap;
    public Tilemap ForbiddenMap;

    public string ThisLevel;
    public string NextLevel;

    public bool Paused { get; private set; }
    public bool MenuOpen { get; private set; }
    public float GameSpeed { get; private set; }

    public static readonly float TimeStep = 0.1f;

    public static Dictionary<string, KeyCode> KeyBinds = new Dictionary<string, KeyCode>
    {
        { "Repair", KeyCode.R },
        { "Fast Forward", KeyCode.F },
        { "Toggle Menu", KeyCode.Escape },
        { "Toggle Pause", KeyCode.Space }
    };

    private Cursor cursor;

    private bool fastForwarding;

    private Dictionary<Vector3Int, Health> repairableObjects;
    private Dictionary<Vector3Int, int> repairableHealth;
    private MarketManager marketManager;
    private SceneLoader sceneLoader;

    private GameState currentState;
    private enum GameState
    {
        Start,
        Playing,
        Paused,
        Over,
        Won,
        Menu
    }

    protected void Awake()
    {
        // Set up initial game state
        currentState = GameState.Start;
        Paused = true;
        MenuOpen = false;
        fastForwarding = false;
        GameSpeed = 1f;

        if (Dirt == null)
            Debug.LogError($"{name} no dirt assigned!");

        if (LevelMap == null)
            Debug.LogError($"{name}: no level map assigned!");

        if (ForbiddenMap == null)
            Debug.LogError($"{name}: no forbidden map assigned!");

        cursor = FindObjectOfType<Cursor>();

        if (cursor == null)
            Debug.LogError($"{name}: unable to find cursor!");

        marketManager = FindObjectOfType<MarketManager>();

        if (marketManager == null)
            Debug.LogError($"{name}: unable to find market manager!");

        sceneLoader = FindObjectOfType<SceneLoader>();

        if (sceneLoader == null)
            Debug.LogError($"{name}: unable to find scene loader!");

        // Initializing the object dictionary list
        repairableObjects = new Dictionary<Vector3Int, Health>();
        repairableHealth = new Dictionary<Vector3Int, int>();

        // Register listeners
        BaseDamageEvent.RegisterListener(DamageTaken);
        TileDestroyedEvent.RegisterListener(TileDestroyed);
        EnemyRecycledEvent.RegisterListener(EnemyRecycled);
        GameWonEvent.RegisterListener(GameWon);

    }

    protected void OnDestroy()
    {
        BaseDamageEvent.UnregisterListener(DamageTaken);
        TileDestroyedEvent.UnregisterListener(TileDestroyed);
        EnemyRecycledEvent.UnregisterListener(EnemyRecycled);
        GameWonEvent.UnregisterListener(GameWon);
    }

    protected void Update()
    {
        // Handle repairing
        if (Input.GetKeyDown(KeyBinds["Repair"]))
            RepairAll();

        // Handle fast forwarding
        if (Input.GetKeyDown(KeyBinds["Fast Forward"]) && currentState == GameState.Playing)
            ToggleFastForward();

        // Handle pausing
        if (Input.GetKeyDown(KeyBinds["Toggle Pause"]))
        {
            if (currentState == GameState.Playing)
                Pause();
            else if (currentState == GameState.Paused)
                UnPause();
        }

        // Handle menu toggling
        if (Input.GetKeyDown(KeyBinds["Toggle Menu"]))
        {
            if (MenuOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    /// <summary>
    /// Called when level starts. Responsible for broadcasting the AssaultStartEvent.
    /// </summary>
    public void StartAssault()
    {
        if (currentState != GameState.Start)
        {
            Debug.LogWarning($"Game state warning! Attempting to start level from {currentState}!");
            return;
        }

        currentState = GameState.Paused;

        GameStartEvent e = new GameStartEvent
        {
            Description = $"Assault has started!"
        };

        e.TriggerEvent();
    }

    /// <summary>
    /// Called whenever the game is paused.
    /// </summary>
    public void Pause()
    {
        if (currentState != GameState.Playing)
        {
            Debug.LogWarning($"Game state warning! Attempting to pause game from {currentState}!");
            return;
        }

        currentState = GameState.Paused;
        Paused = true;

        PauseEvent e = new PauseEvent
        {
            Description = "Game paused",
            Paused = true
        };
        e.TriggerEvent();
    }

    /// <summary>
    /// Called whenever play is resumed.
    /// </summary>
    public void UnPause()
    {
        if (currentState != GameState.Paused)
        {
            Debug.LogWarning($"Game state warning! Attempting to unpause game from {currentState}!");
            return;
        }

        currentState = GameState.Playing;
        Paused = false;

        PauseEvent e = new PauseEvent
        {
            Description = "Game unpaused",
            Paused = false
        };
        e.TriggerEvent();
    }

    /// <summary>
    /// Called whenever the in-game menu is opened.
    /// </summary>
    public void OpenMenu()
    {
        if (currentState != GameState.Paused && currentState != GameState.Playing)
        {
            Debug.LogWarning($"Game state warning! Attempting to open menu from {currentState}!");
            return;
        }

        if (currentState == GameState.Playing)
            Pause();

        currentState = GameState.Menu;
        MenuOpen = true;

        MenuEvent e = new MenuEvent
        {
            Description = "Menu was opened.",
            MenuOpen = MenuOpen
        };
        e.TriggerEvent();
    }

    /// <summary>
    /// Called whenever the in-game menu is closed.
    /// </summary>
    public void CloseMenu()
    {
        if (currentState != GameState.Menu)
        {
            Debug.LogWarning($"Game state warning! Attempting to close menu from {currentState}!");
            return;
        }

        currentState = GameState.Paused;
        MenuOpen = false;

        MenuEvent e = new MenuEvent
        {
            Description = "Menu was closed.",
            MenuOpen = MenuOpen
        };
        e.TriggerEvent();
    }

    /// <summary>
    /// Called when the game is lost.
    /// </summary>
    public void GameOver()
    {
        if (currentState != GameState.Playing)
        {
            Debug.LogWarning($"Game state warning! Attempting to end game from {currentState}!");
            return;
        }

        GameOverEvent gameOver = new GameOverEvent
        {
            Description = $"Game ended. Player lost with {BaseHealth} health remaining."
        };

        gameOver.TriggerEvent();

        currentState = GameState.Over;
        Paused = true;

        PauseEvent e = new PauseEvent
        {
            Description = "Game over",
            Paused = true
        };
        e.TriggerEvent();
    }

    /// <summary>
    /// Called when the game is won.
    /// </summary>
    public void GameWon(GameWonEvent e)
    {
        if (currentState != GameState.Playing)
        {
            Debug.LogWarning($"Game state warning! Attempting to win game from {currentState}!");
            return;
        }

        currentState = GameState.Won;
        Paused = true;

        PauseEvent pauseEvent = new PauseEvent
        {
            Description = "Game won",
            Paused = true
        };
        pauseEvent.TriggerEvent();
    }

    /// <summary>
    /// Called when the fast forwarding button is pressed.
    /// </summary>
    public void ToggleFastForward()
    {
        if (fastForwarding)
            GameSpeed = 1f;
        else
            GameSpeed = 2f;

        fastForwarding = !fastForwarding;

        FastForwardEvent e = new FastForwardEvent
        {
            Description = $"Game has been {(fastForwarding ? "fast-forwarded" : "slowed")}.",
            FastForwarding = fastForwarding,
            GameSpeed = GameSpeed
        };
        e.TriggerEvent();
    }

    public void RepairAll()
    {
        foreach(KeyValuePair<Vector3Int, Health> obj in repairableObjects)
        {
            int price = marketManager.PriceByTile(LevelMap.GetTile(obj.Key));
            float pricePerUnit = (float)price / obj.Value.MaxHealth * 1.5f;
            int totalPrice = Mathf.CeilToInt((obj.Value.MaxHealth - obj.Value.CurrentHealth) * pricePerUnit);

            if (Parts >= totalPrice)
            {
                Parts -= totalPrice;

                PartsChangedUIEvent uiEvent = new PartsChangedUIEvent
                {
                    Description = $"There are now {Parts} parts remaining."
                };

                uiEvent.TriggerEvent();

                obj.Value.Repair();
            }
        }
    }

    public void AddRepairable(Vector3Int cellLoc, Health health)
    {
        if (repairableObjects.ContainsKey(cellLoc))
        {
            repairableObjects[cellLoc] = health;
            repairableHealth[cellLoc] = health.CurrentHealth;
        } else
        {
            repairableObjects.Add(cellLoc, health);
            repairableHealth.Add(cellLoc, health.CurrentHealth);
        }
    }

    public void UpdateHealth(Vector3Int cellLoc)
    {
        if (repairableHealth.ContainsKey(cellLoc))
            repairableHealth[cellLoc] = repairableObjects[cellLoc].CurrentHealth;
    }

    public int GetHealth(Vector3Int cellLoc)
    {
        return repairableHealth.ContainsKey(cellLoc) ? repairableHealth[cellLoc] : -1;
    }

    /// <summary>
    /// Called when the player places an object.
    /// </summary>
    /// <param name="objectToPlace">The object that is getting placed.</param>
    public void PlaceObject(MarketItem objectToPlace, bool second)
    {
        LevelMap.SetTile(cursor.MouseGridPos, second ? objectToPlace.SecondTile : objectToPlace.Tile);

        TileUpdateEvent e = new TileUpdateEvent
        {
            Description = $"Tile {objectToPlace} placed at {cursor.MouseGridPos}."
        };
        e.TriggerEvent();

        if (!second)
        {
            Parts -= objectToPlace.Price;

            PartsChangedUIEvent uiEvent = new PartsChangedUIEvent
            {
                Description = $"There are now {Parts} parts remaining."
            };

            uiEvent.TriggerEvent();
        }
    }

    /// <summary>
    /// Called when the player sells an object.
    /// </summary>
    /// <param name="location">The location of the object getting sold.</param>
    public void SellObject(Vector3Int location)
    {
        int value = marketManager.PriceByTile(LevelMap.GetTile(location));

        if (value == 0)
        {
            Debug.LogError($"{name}: trying to sell tile that has no value!");
            return;
        }

        int maxHealth = repairableObjects.ContainsKey(location) ? repairableObjects[location].MaxHealth : -1;
        int health = repairableHealth.ContainsKey(location) ? repairableHealth[location] : -1;

        int returnValue = Mathf.RoundToInt((float)value * health / maxHealth * Return);

        Parts += returnValue;

        PartsChangedUIEvent uiEvent = new PartsChangedUIEvent
        {
            Description = $"There are now {Parts} parts remaining."
        };

        uiEvent.TriggerEvent();

        LevelMap.SetTile(location, Dirt);
        ForbiddenMap.SetTile(location, null);
        repairableObjects.Remove(location);
        repairableHealth.Remove(location);

        TileSoldEvent e = new TileSoldEvent
        {
            Description = $"Tile at {(Vector2Int)location} sold for {returnValue} parts.",
            CellLoc = (Vector2Int)location,
            ReturnValue = returnValue
        };
        e.TriggerEvent();
    }

    // Called to start level
    public void StartGame()
    {
        sceneLoader.LoadLevel(ThisLevel);
    }

    // Called to start the next level
    public void LoadNextLevel()
    {
        sceneLoader.LoadLevel(NextLevel);
    }

    // Called when the user wants to quit the application
    public void Quit()
    {
        Application.Quit();
    }

    private void TileDestroyed(TileDestroyedEvent e)
    {
        LevelMap.SetTile((Vector3Int)e.CellLoc, Dirt);
        ForbiddenMap.SetTile((Vector3Int)e.CellLoc, null);
        repairableObjects.Remove((Vector3Int)e.CellLoc);
        repairableHealth.Remove((Vector3Int)e.CellLoc);

        TileUpdateEvent tileUpdated = new TileUpdateEvent
        {
            Description = $"{name} was destroyed."
        };

        tileUpdated.TriggerEvent();
    }

    /// <summary>
    /// Triggered when damage is taken.
    /// </summary>
    /// <param name="e">Event parameter containing details on the damage that was dealt.</param>
    private void DamageTaken(BaseDamageEvent e)
    {
        BaseHealth -= e.DamageAmount;

        BaseDamageUIEvent uiEvent = new BaseDamageUIEvent
        {
            Description = $"Base now has {BaseHealth}"
        };

        uiEvent.TriggerEvent();

        if (BaseHealth <= 0)
            GameOver();
    }

    private void EnemyRecycled(EnemyRecycledEvent e)
    {
        Parts += e.Value;

        PartsChangedUIEvent uiEvent = new PartsChangedUIEvent
        {
            Description = $"There are now {Parts} parts remaining."
        };

        uiEvent.TriggerEvent();
    }

}
