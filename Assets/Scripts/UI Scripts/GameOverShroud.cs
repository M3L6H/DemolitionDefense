using UnityEngine;
using EventCallbacks;

public class GameOverShroud : MonoBehaviour
{

    protected void Awake()
    {
        gameObject.SetActive(false);

        // Set up listeners
        GameOverEvent.RegisterListener(GameOver);
    }

    private void GameOver(GameOverEvent e)
    {
        gameObject.SetActive(true);
    }

    protected void OnDestroy()
    {
        GameOverEvent.UnregisterListener(GameOver);
    }

}
