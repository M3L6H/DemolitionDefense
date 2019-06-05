using UnityEngine;

namespace EventCallbacks
{
    public abstract class Event<T> where T : Event<T>
    {

        public string Description;
        public int LoggingLevel { get; protected set; }

        public delegate void EventListener(T info);
        private static event EventListener listeners;

        public static void RegisterListener(EventListener listener)
        {
            listeners += listener;
        }

        public static void UnregisterListener(EventListener listener)
        {
            listeners -= listener;
        }

        public void TriggerEvent()
        {
            listeners?.Invoke(this as T);
        }

    }

    public class PauseEvent : Event<PauseEvent>
    {

        public bool Paused;

        public PauseEvent()
        {
            LoggingLevel = 2;
        }

    }

    public class TileSoldEvent : Event<TileSoldEvent>
    {

        public Vector2Int CellLoc;
        public int ReturnValue;

        public TileSoldEvent()
        {
            LoggingLevel = 3;
        }

    }

    public class TileDestroyedEvent : Event<TileDestroyedEvent>
    {

        public Vector2Int CellLoc;

        public TileDestroyedEvent()
        {
            LoggingLevel = 3;
        }

    }

    public class TileDamageEvent : Event<TileDamageEvent>
    {

        public Vector2Int CellLoc;
        public int DamageAmount;
        public Enemy enemy;

        public TileDamageEvent()
        {
            LoggingLevel = 4;
        }

    }

    public class TileUpdateEvent : Event<TileUpdateEvent>
    {

        public TileUpdateEvent()
        {
            LoggingLevel = 4;
        }

    }

    public class BaseDamageEvent : Event<BaseDamageEvent>
    {

        public int DamageAmount;

        public BaseDamageEvent()
        {
            LoggingLevel = 3;
        }

    }

    public class BaseDamageUIEvent : Event<BaseDamageUIEvent>
    {

        public BaseDamageUIEvent()
        {
            LoggingLevel = 5;
        }

    }

    public class PurchaseMadeEvent : Event<PurchaseMadeEvent>
    {

        public int Cost;

        public PurchaseMadeEvent()
        {
            LoggingLevel = 3;
        }

    }

    public class PartsChangedUIEvent : Event<PartsChangedUIEvent>
    {

        public PartsChangedUIEvent()
        {
            LoggingLevel = 5;
        }

    }

    public class EnemyRecycledEvent : Event<EnemyRecycledEvent>
    {

        public int Value;

        public EnemyRecycledEvent()
        {
            LoggingLevel = 3;
        }

    }

    public class GameStartEvent : Event<GameStartEvent>
    {

        public GameStartEvent()
        {
            LoggingLevel = 1;
        }

    }

    public class GameOverEvent : Event<GameOverEvent>
    {

        public GameOverEvent()
        {
            LoggingLevel = 1;
        }

    }

    public class GameWonEvent : Event<GameWonEvent>
    {

        public GameWonEvent()
        {
            LoggingLevel = 1;
        }

    }

    public class FastForwardEvent : Event<FastForwardEvent>
    {

        public bool FastForwarding;
        public float GameSpeed;

        public FastForwardEvent()
        {
            LoggingLevel = 2;
        }

    }

}
