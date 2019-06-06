using UnityEngine;
using UnityEngine.UI;
using EventCallbacks;

public class MenuButton : MonoBehaviour
{

    private Button button;

    protected void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
            Debug.LogError($"{name}: unable to find button component!");

        // Register listeners
        GameStartEvent.RegisterListener(GameStart);
        GameOverEvent.RegisterListener(GameOver);
        MenuEvent.RegisterListener(Menu);
    }

    protected void Start()
    {
        button.interactable = false;
    }

    private void GameStart(GameStartEvent e)
    {
        button.interactable = true;
    }

    private void GameOver(GameOverEvent e)
    {
        button.interactable = false;
    }

    private void Menu(MenuEvent e)
    {
        button.interactable = !e.MenuOpen;
    }

    protected void OnDestroy()
    {
        GameOverEvent.UnregisterListener(GameOver);
        GameStartEvent.UnregisterListener(GameStart);
        MenuEvent.UnregisterListener(Menu);
    }

}
