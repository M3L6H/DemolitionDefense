using UnityEngine;
using UnityEngine.UI;
using EventCallbacks;

public class PauseButton : MonoBehaviour
{

    private Button button;

    protected void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
            Debug.LogError($"{name} does not have an attached button component!");

        // Register listeners
        PauseEvent.RegisterListener(GamePaused);
    }

    private void GamePaused(PauseEvent e)
    {
        button.interactable = !e.Paused;
    }

}
