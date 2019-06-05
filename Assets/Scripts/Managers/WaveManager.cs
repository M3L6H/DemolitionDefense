using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections;
using EventCallbacks;

[System.Serializable]
public class WaveManager : MonoBehaviour
{

    public Tilemap LevelMap;

    public float SpawnSpeed;
    public float WaveCooldown;
    public WaveEntry[] Waves;

    private GameManager gm;

    private readonly float timeStep = 0.1f;

    protected void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: cannot find game manager!");

        if (LevelMap == null)
            Debug.LogError($"{name}: No level map assigned!");

        // Register listeners
        GameStartEvent.RegisterListener(StartAssault);
    }

    /// <summary>
    /// Called when the level begins.
    /// </summary>
    /// <param name="e">Details about the beginning of the event.</param>
    private void StartAssault(GameStartEvent e)
    {
        StartCoroutine(WaveClockCoroutine());
    }

    /// <summary>
    /// Called when the player has defeated all the waves.
    /// </summary>
    private void EndAssault()
    {
        GameWonEvent e = new GameWonEvent
        {
            Description = "Player defeated all waves. Game won."
        };
        e.TriggerEvent();
    }

    private IEnumerator WaitForLastEnemy()
    {
        while (FindObjectOfType<Enemy>() != null)
            yield return new WaitForSeconds(timeStep * 5);

        EndAssault();
    }

    // Main clock that iterates through the waves
    private IEnumerator WaveClockCoroutine()
    {
        for (int i = 0;  i < Waves.Length; i++)
        {
            if (gm.Paused)
            {
                yield return new WaitForSeconds(0.1f);
                i--;
                continue;
            }
            yield return StartCoroutine(SpawnWaveCoroutine(i));

            // Wait for alloted amount of time
            float timeElapsed = 0f;

            while (timeElapsed <= WaveCooldown / gm.GameSpeed)
            {
                if (!gm.Paused)
                    timeElapsed += timeStep;
                yield return new WaitForSeconds(timeStep);
            }
        }

        StartCoroutine(WaitForLastEnemy());
    }

    // Responsible for spawning each of the enemies within a wave
    private IEnumerator SpawnWaveCoroutine(int i)
    {
        int j = 0;
        while (Waves[i].Enemies.Length > j)
        {
            if (gm.Paused)
            {
                yield return new WaitForSeconds(0.1f);
                i--;
                continue;
            }
            Enemy e = Instantiate(Waves[i].Enemies[j], Waves[i].Sources[j].transform.position, Quaternion.identity);
            e.GetComponent<Pathing>().LevelMap = LevelMap;
            e.name = $"Enemy (W{i} - U{j})";

            // Wait for alloted amount of time
            float timeElapsed = 0f;

            while (timeElapsed <= SpawnSpeed / gm.GameSpeed)
            {
                if (!gm.Paused)
                    timeElapsed += timeStep;
                yield return new WaitForSeconds(timeStep);
            }

            // Progress through the enemy list
            j++;
        }
    }

}

[System.Serializable]
public class WaveEntry
{

    public Enemy[] Enemies;
    public Source[] Sources;

}