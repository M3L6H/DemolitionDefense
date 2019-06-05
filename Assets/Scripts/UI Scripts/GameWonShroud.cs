using UnityEngine;
using EventCallbacks;

public class GameWonShroud : MonoBehaviour
{

    protected void Awake()
    {
        // Register listeners
        GameWonEvent.RegisterListener(GameWon);
    }

    protected void Start()
    {
        gameObject.SetActive(false);
    }

    private void GameWon(GameWonEvent e)
    {
        gameObject.SetActive(true);
    }

}
