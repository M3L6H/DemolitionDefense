using EventCallbacks;
using UnityEngine;

public class ScrollPanel : MonoBehaviour
{

    public float DurationPerChar = 0.15f;
    public float WaitTime = 0.75f;

    public ScrollText ScrollTextPrefab;

    public string[] Text;

    private int index;

    private void Awake()
    {
        if (ScrollTextPrefab == null)
            Debug.LogError($"{name}: No scroll text prefab assigned!");

        TextScrollEvent.RegisterListener(ShowTextEvent);
        index = 0;
    }

    private void OnDestroy()
    {
        TextScrollEvent.UnregisterListener(ShowTextEvent);
    }

    private void Start()
    {
        ShowTextEvent(new TextScrollEvent() { Hidden = true });
    }

    private void ShowTextEvent(TextScrollEvent e)
    {
        ScrollText text = Instantiate(ScrollTextPrefab, transform);
        text.SetText(Text[index]);
        text.Show(Text[index].Length * DurationPerChar, WaitTime);
        index = (index + 1) % Text.Length;
    }

}

