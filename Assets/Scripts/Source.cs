using UnityEngine;
using EventCallbacks;

[System.Serializable]
public class Source : MonoBehaviour
{

    private Pathing pathing;

    protected void Awake()
    {
        pathing = GetComponent<Pathing>();

        if (pathing == null)
            Debug.LogError(name + ": No pathing component attached!");

        // Register listener
        TileUpdateEvent.RegisterListener(TileUpdated);
    }

    protected void Start()
    {
        pathing.CalculatePath();
    }

    // We need to recalculate the path every time a tile is changed
    private void TileUpdated(TileUpdateEvent e)
    {
        pathing.CalculatePath();
    }

}
