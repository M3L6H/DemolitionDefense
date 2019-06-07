using UnityEngine;
using EventCallbacks;

public class PauseShroud : MonoBehaviour
{

    protected void Awake()
    {
        // Register listeners
        PauseEvent.RegisterListener(Pause);
    }

    protected void OnDestroy()
    {
        PauseEvent.UnregisterListener(Pause);
    }

    protected void Start()
    {
        gameObject.SetActive(false);
    }

    private void Pause(PauseEvent e)
    {
        gameObject.SetActive(e.Paused);
    }

}
