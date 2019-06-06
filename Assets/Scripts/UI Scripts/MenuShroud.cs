using UnityEngine;
using EventCallbacks;

public class MenuShroud : MonoBehaviour
{

    protected void Awake()
    {
        // Register listeners
        MenuEvent.RegisterListener(Menu);
    }

    protected void Start()
    {
        gameObject.SetActive(false);
    }

    private void Menu(MenuEvent e)
    {
        gameObject.SetActive(e.MenuOpen);
    }

    protected void OnDestroy()
    {
        MenuEvent.UnregisterListener(Menu);
    }

}
