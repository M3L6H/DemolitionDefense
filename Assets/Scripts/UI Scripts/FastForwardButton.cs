using UnityEngine;
using UnityEngine.UI;
using EventCallbacks;

public class FastForwardButton : MonoBehaviour
{

    private Image buttonImage;

    protected void Awake()
    {
        buttonImage = GetComponent<Image>();

        if (buttonImage == null)
            Debug.LogError($"{name} does not have an attached button component!");

        // Register listeners
        FastForwardEvent.RegisterListener(FastForwarded);
    }

    private void FastForwarded(FastForwardEvent e)
    {
        if (e.FastForwarding)
            buttonImage.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        else
            buttonImage.color = Color.white;
    }

}
