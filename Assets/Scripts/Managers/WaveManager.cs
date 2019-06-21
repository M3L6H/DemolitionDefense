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
    private WaveText waveText;
    private WaveCounter waveCounter;

    protected void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: cannot find game manager!");

        waveText = FindObjectOfType<WaveText>();

        if (waveText == null)
            Debug.LogError($"{name}: cannot find wave text!");

        waveCounter = FindObjectOfType<WaveCounter>();

        if (waveCounter == null)
            Debug.LogError($"{name}: cannot find wave counter!");

        if (LevelMap == null)
            Debug.LogError($"{name}: No level map assigned!");

        // Register listeners
        GameStartEvent.RegisterListener(StartAssault);
    }

    protected void OnDestroy()
    {
        GameStartEvent.UnregisterListener(StartAssault);
    }

    /// <summary>
    /// Called when the level begins.
    /// </summary>
    /// <param name="e">Details about the beginning of the event.</param>
    private void StartAssault(GameStartEvent e)
    {
        waveCounter.SetCount(Waves.Length);
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
            yield return new WaitForSeconds(GameManager.TimeStep * 5);
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
            waveText.ShowText((i == Waves.Length - 1) ? "Last Wave" : $"Wave {i + 1}");
            waveCounter.ShowWaves();
            waveCounter.IncrementWave();

            // Wait for alloted amount of time
            float timeElapsed = 0f;

            while (timeElapsed <= WaveCooldown / gm.GameSpeed)
            {
                if (!gm.Paused)
                    timeElapsed += GameManager.TimeStep;
                yield return new WaitForSeconds(GameManager.TimeStep);
            }

            yield return StartCoroutine(SpawnWaveCoroutine(i));
            yield return StartCoroutine(WaitForLastEnemy());
            gm.WaveOver(Waves[i].WaveReward);
        }

        yield return StartCoroutine(WaitForLastEnemy());

        EndAssault();
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
                    timeElapsed += GameManager.TimeStep;
                yield return new WaitForSeconds(GameManager.TimeStep);
            }

            // Progress through the enemy list
            j++;
        }
    }

}

[System.Serializable]
public class WaveEntry
{

    public int WaveReward;
    public Enemy[] Enemies;
    public Source[] Sources;

}