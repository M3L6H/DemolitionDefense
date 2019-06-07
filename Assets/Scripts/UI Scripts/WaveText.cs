using UnityEngine;
using TMPro;

public class WaveText : MonoBehaviour
{

    private TextMeshProUGUI text;
    private GameManager gm;

    private readonly float[] times = { .5f, .5f, .3f, .3f, .3f, .3f, .1f, .1f, .1f, .1f };

    private bool showing;
    private float timeElapsed;
    private int index;

    protected void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        if (text == null)
            Debug.LogError($"{name}: cannot find text component!");

        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: could not find game manager!");

        showing = false;
        index = 0;
        timeElapsed = 0f;
    }

    protected void Start()
    {
        gameObject.SetActive(false);
        text.color = new Color(1f, 1f, 1f, 0f);
    }

    protected void Update()
    {
        if (gm.Paused)
            return;

        timeElapsed += Time.deltaTime;
        
        if (showing)
        {
            if (timeElapsed >= times[index] / gm.GameSpeed)
            {
                index++;
                text.color = new Color(1f, 1f, 1f, 1f);
                showing = false;
                timeElapsed = 0f;
            } else
            {
                text.color = new Color(1f, 1f, 1f, Mathf.Clamp01(timeElapsed / times[index]));
            }
        } else
        {
            if (timeElapsed >= times[index] / gm.GameSpeed)
            {
                index++;
                text.color = new Color(1f, 1f, 1f, 0f);
                showing = true;
                timeElapsed = 0f;
            }
            else
            {
                text.color = new Color(1f, 1f, 1f, 1f - Mathf.Clamp01(timeElapsed / times[index]));
            }
        }

        if (index >= times.Length)
        {
            gameObject.SetActive(false);
        }
    }

    public void ShowText(string text)
    {
        this.text.text = text;
        gameObject.SetActive(true);
        showing = true;
        index = 0;
        timeElapsed = 0f;
    }

}
