using UnityEngine;
using EventCallbacks;

public class Logger : MonoBehaviour
{

    [Range(0, 5)]
    public int LoggingLevel = 3;

    protected void Awake()
    {
        PauseEvent.RegisterListener(LogEvent);
        MenuEvent.RegisterListener(LogEvent);
        TileSoldEvent.RegisterListener(LogEvent);
        TileDestroyedEvent.RegisterListener(LogEvent);
        TileDamageEvent.RegisterListener(LogEvent);
        TileUpdateEvent.RegisterListener(LogEvent);
        BaseDamageEvent.RegisterListener(LogEvent);
        BaseDamageUIEvent.RegisterListener(LogEvent);
        PurchaseMadeEvent.RegisterListener(LogEvent);
        PartsChangedUIEvent.RegisterListener(LogEvent);
        EnemyRecycledEvent.RegisterListener(LogEvent);
        GameStartEvent.RegisterListener(LogEvent);
        GameOverEvent.RegisterListener(LogEvent);
        GameWonEvent.RegisterListener(LogEvent);
        FastForwardEvent.RegisterListener(LogEvent);
        TextScrollEvent.RegisterListener(LogEvent);
    }

    protected void OnDestroy()
    {
        PauseEvent.UnregisterListener(LogEvent);
        MenuEvent.UnregisterListener(LogEvent);
        TileSoldEvent.UnregisterListener(LogEvent);
        TileDestroyedEvent.UnregisterListener(LogEvent);
        TileDamageEvent.UnregisterListener(LogEvent);
        TileUpdateEvent.UnregisterListener(LogEvent);
        BaseDamageEvent.UnregisterListener(LogEvent);
        BaseDamageUIEvent.UnregisterListener(LogEvent);
        PurchaseMadeEvent.UnregisterListener(LogEvent);
        PartsChangedUIEvent.UnregisterListener(LogEvent);
        EnemyRecycledEvent.UnregisterListener(LogEvent);
        GameStartEvent.UnregisterListener(LogEvent);
        GameOverEvent.UnregisterListener(LogEvent);
        GameWonEvent.UnregisterListener(LogEvent);
        FastForwardEvent.UnregisterListener(LogEvent);
        TextScrollEvent.UnregisterListener(LogEvent);
    }

    private void LogEvent(PauseEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(MenuEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(TileSoldEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(TileDestroyedEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(TileDamageEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(TileUpdateEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(BaseDamageEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(BaseDamageUIEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(PurchaseMadeEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(PartsChangedUIEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(EnemyRecycledEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(GameStartEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(GameOverEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(GameWonEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(FastForwardEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

    private void LogEvent(TextScrollEvent e)
    {
        if (e.LoggingLevel <= LoggingLevel)
            Debug.Log(e.Description);
    }

}
