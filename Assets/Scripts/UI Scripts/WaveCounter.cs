using UnityEngine;
using TMPro;

public class WaveCounter : MonoBehaviour
{

    private TextMeshProUGUI text;
    private GameManager gm;

    private int currentWave;
    private int waveCount;

    protected void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        if (text == null)
            Debug.LogError($"{name}: cannot find text component!");

        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: could not find game manager!");
    }

    protected void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetCount(int count)
    {
        waveCount = count;
        ResetWaves();
    }

    public void ResetWaves()
    {
        currentWave = 1;
    }

    public void IncrementWave()
    {
        currentWave++;
    }

    public void ShowWaves()
    {
        text.text = $"{currentWave}/{waveCount}";
        gameObject.SetActive(true);
    }

}
