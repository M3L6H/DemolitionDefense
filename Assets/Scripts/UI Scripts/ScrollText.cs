using EventCallbacks;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScrollText : MonoBehaviour
{

    public float fadeIn = 1f;
    public float fadeOut = 1f;

    private TextMeshProUGUI text;

    protected void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        if (text == null)
            Debug.LogError($"{name}: No text component found!");
    }

    protected void Start()
    {
        SetAlpha(0f);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void Show(float showFor, float waitTime)
    {
        StartCoroutine(ShowCoroutine(showFor, waitTime));
    }

    private IEnumerator ShowCoroutine(float showFor, float waitTime)
    {
        // Fade in
        float timeElapsed = 0f;

        while (timeElapsed <= fadeIn)
        {
            yield return new WaitForSeconds(GameManager.TimeStep);
            timeElapsed += GameManager.TimeStep;
            SetAlpha(timeElapsed / fadeIn);
        }

        // Show text
        timeElapsed = 0f;

        while (timeElapsed <= showFor)
        {
            yield return new WaitForSeconds(GameManager.TimeStep);
            timeElapsed += GameManager.TimeStep;
        }

        // Fade out
        timeElapsed = 0f;

        while (timeElapsed <= fadeOut)
        {
            yield return new WaitForSeconds(GameManager.TimeStep);
            timeElapsed += GameManager.TimeStep;
            SetAlpha((fadeOut - timeElapsed) / fadeOut);
        }

        // Wait
        timeElapsed = 0f;

        while (timeElapsed <= waitTime)
        {
            yield return new WaitForSeconds(GameManager.TimeStep);
            timeElapsed += GameManager.TimeStep;
        }

        // Text event
        TextScrollEvent e = new TextScrollEvent()
        {
            Description = $"Text scroll complete: {text.text}",
            Hidden = true
        };
        e.TriggerEvent();
        Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        text.color = new Color(text.color.r,
                               text.color.g,
                               text.color.b,
                               alpha);
    }

}
