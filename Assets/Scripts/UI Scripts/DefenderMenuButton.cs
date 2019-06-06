using UnityEngine;
using UnityEngine.UI;
using EventCallbacks;

public class DefenderMenuButton : MonoBehaviour
{

    public float Sensitivity = 100f;
    public float Smoothness = 0.5f;
    [Range(0,1)]
    public float HiddenAlpha = 0.1f;

    public Sprite OpenMenu;
    public Sprite CloseMenu;

    public bool Disabled { get; private set; }

    private Image image;
    private Button button;
    private RectTransform rect;
    private float defaultY;
    private float targetY;
    private DefenderMenu defenderMenu;

    private bool toggle = false;

    protected void Awake()
    {
        defenderMenu = FindObjectOfType<DefenderMenu>();

        if (defenderMenu == null)
            Debug.LogError($"{name}: cannot find defender menu!");

        if (OpenMenu == null || CloseMenu == null)
            Debug.LogError($"{name}: does not have sprites assigned!");

        rect = GetComponent<RectTransform>();

        if (rect == null)
            Debug.LogError($"{name}: cannot find its rect transform!");

        image = GetComponent<Image>();

        if (image == null)
            Debug.LogError($"{name}: cannot find image!");

        button = GetComponent<Button>();

        if (button == null)
            Debug.LogError($"{name}: cannot find button!");

        button.onClick.AddListener(ToggleMenu);

        Disabled = true;

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

    public void ToggleMenu()
    {
        SetMenu(!toggle);
    }

    public void SetMenu(bool set)
    {
        toggle = set;
        defenderMenu.ToggleMenu(toggle);
    }

    protected void Start()
    {
        // Set up initial positions
        defaultY = rect.localPosition.y;
        targetY = rect.localPosition.y - defenderMenu.Rect.sizeDelta.y;

        // Fade out button
        Color color = image.color;
        color.a = HiddenAlpha;
        image.color = color;

        // Disable button
        button.interactable = false;
    }

    protected void Update()
    {
        if (!toggle)
            image.overrideSprite = OpenMenu;
        else
            image.overrideSprite = CloseMenu;

        Vector3 pos = rect.localPosition;
        pos.y = toggle ? targetY : defaultY;
        rect.localPosition = Vector3.Lerp(rect.localPosition, pos, Smoothness);

        if (Vector2.Distance(Input.mousePosition, transform.position) < Sensitivity && !Disabled)
        {
            // Fade in button
            Color color = image.color;
            color.a = 1f;
            image.color = Color.Lerp(image.color, color, Smoothness);

            // Enable button
            button.interactable = true;
        } else
        {
            // Fade out button
            Color color = image.color;
            color.a = HiddenAlpha;
            image.color = Color.Lerp(image.color, color, Smoothness);

            // Disable button
            button.interactable = false;
        }

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
