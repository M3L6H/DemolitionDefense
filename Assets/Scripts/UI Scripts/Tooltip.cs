using EventCallbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{

    public float xShift = 0f;
    public float yShift = 0f;

    private RectTransform parent;
    private Image image;
    private TextMeshProUGUI child;

    public bool Disabled { get; private set; }

    private void Awake()
    {
        parent = transform.parent.GetComponent<RectTransform>();

        if (parent == null)
            Debug.LogError($"{name}: parent rect transform not found!");

        image = GetComponent<Image>();

        if (image == null)
            Debug.LogError($"{name}: could not find image component!");

        child = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (child == null)
            Debug.LogError($"{name}: child text component not found!");

        Disabled = true;
        image.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);

        // Register event listeners
        GameOverEvent.RegisterListener(GameOver);
        MenuEvent.RegisterListener(Menu);
        GameStartEvent.RegisterListener(GameStart);
    }

    protected void OnDestroy()
    {
        GameOverEvent.UnregisterListener(GameOver);
        MenuEvent.UnregisterListener(Menu);
        GameStartEvent.UnregisterListener(GameStart);
    }

    private void Update()
    {
        if (Vector2.Distance(Input.mousePosition, parent.transform.position + new Vector3(xShift, yShift, 0f)) < parent.rect.size.x * 0.6f && !Disabled)
        {
            image.enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            image.enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void SetText(string text)
    {
        child.text = text;
    }

    private void GameStart(GameStartEvent e)
    {
        Disabled = false;
    }

    private void GameOver(GameOverEvent e)
    {
        Disabled = true;
    }

    private void Menu(MenuEvent e)
    {
        Disabled = e.MenuOpen;
    }

}
