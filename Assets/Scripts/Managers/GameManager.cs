﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using EventCallbacks;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public int BaseHealth;
    public int Parts;

    public float Return = 0.5f;

    public TileBase Dirt;
    public Tilemap LevelMap;
    public Tilemap ForbiddenMap;

    public Scene NextLevel;
    private LoadingScreen loadingScreen;

    public bool Paused { get; private set; }
    public float GameSpeed { get; private set; }

    public static Dictionary<string, KeyCode> KeyBinds = new Dictionary<string, KeyCode>
    {
        { "Toggle Pause", KeyCode.Space },
        { "Fast Forward", KeyCode.F }
    };

    private Cursor cursor;

    private bool fastForwarding;

    private Dictionary<Vector3Int, Health> repairableObjects;
    private Dictionary<Vector3Int, int> repairableHealth;
    private MarketManager marketManager;

    private GameState currentState;
    private enum GameState
    {
        Start,
        Playing,
        Paused,
        Over,
        Won
    }

    protected void Awake()
    {
        // Set up initial game state
        currentState = GameState.Start;
        Paused = true;
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

        loadingScreen = FindObjectOfType<LoadingScreen>();

        if (loadingScreen == null)
            Debug.LogError($"{name}: unable to find loading screen!");

        marketManager = FindObjectOfType<MarketManager>();

        if (marketManager == null)
            Debug.LogError($"{name}: unable to find market manager!");

        // Initializing the object dictionary list
        repairableObjects = new Dictionary<Vector3Int, Health>();
        repairableHealth = new Dictionary<Vector3Int, int>();

        // Register listeners
        BaseDamageEvent.RegisterListener(DamageTaken);
        TileDestroyedEvent.RegisterListener(TileDestroyed);
        EnemyRecycledEvent.RegisterListener(EnemyRecycled);
        GameWonEvent.RegisterListener(GameWon);
    }

    protected void Update()
    {
        // HACK: Bind this to the menu
        if (Input.GetKeyDown(KeyCode.Escape))
            Quit();

        // Handle pausing
        if (Input.GetKeyDown(KeyBinds["Toggle Pause"]))
        {
            if (currentState == GameState.Playing)
                Pause();
            else if (currentState == GameState.Paused)
                UnPause();
        }

        // Handle fast forwarding
        if (Input.GetKeyDown(KeyBinds["Fast Forward"]) && currentState == GameState.Playing)
            ToggleFastForward();
    }

    /// <summary>
    /// Called when level starts. Responsible for broadcasting the AssaultStartEvent.
    /// </summary>
    public void StartAssault()
    {
        if (currentState != GameState.Start)
        {
            Debug.LogError($"Game state error! Attempting to start level from {currentState}!");
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
            Debug.LogError($"Game state error! Attempting to pause game from {currentState}!");
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
            Debug.LogError($"Game state error! Attempting to unpause game from {currentState}!");
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
    /// Called when the game is lost.
    /// </summary>
    public void GameOver()
    {
        if (currentState != GameState.Playing)
        {
            Debug.LogError($"Game state error! Attempting to end game from {currentState}!");
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
            Debug.LogError($"Game state error! Attempting to win game from {currentState}!");
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
    public void PlaceObject(MarketItem objectToPlace)
    {
        Parts -= objectToPlace.Price;
        LevelMap.SetTile(cursor.MouseGridPos, objectToPlace.Tile);

        TileUpdateEvent e = new TileUpdateEvent
        {
            Description = $"Tile {objectToPlace} placed at {cursor.MouseGridPos}."
        };
        e.TriggerEvent();

        PartsChangedUIEvent uiEvent = new PartsChangedUIEvent
        {
            Description = $"There are now {Parts} parts remaining."
        };

        uiEvent.TriggerEvent();
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

    /// <summary>
    /// Called to load the subsequent level. Defers to an asynchronous load function.
    /// </summary>
    public void LoadNextLevel()
    {
        if (NextLevel == null)
        {
            Debug.LogError("No next level defined!");
            return;
        }

        loadingScreen.gameObject.SetActive(true);
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(NextLevel.buildIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // HACK: Need a separate class for this 
    // Called from the main menu to start the game
    public void StartGame()
    {
        SceneManager.LoadScene("Development");
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
