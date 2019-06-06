using UnityEngine;
using UnityEngine.UI;
using EventCallbacks;

public class PlayButton : MonoBehaviour
{

    private Button button;

    protected void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
            Debug.LogError($"{name} does not have an attached button component!");

        // Register listeners
        PauseEvent.RegisterListener(GamePaused);
        MenuEvent.RegisterListener(Menu);
    }

    private void GamePaused(PauseEvent e)
    {
        button.interactable = e.Paused;
    }

    private void Menu(MenuEvent e)
    {
        button.interactable = !e.MenuOpen;
    }

    protected void OnDestroy()
    {
        PauseEvent.UnregisterListener(GamePaused);
        MenuEvent.UnregisterListener(Menu);
    }

}
