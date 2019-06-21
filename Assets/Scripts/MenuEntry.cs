using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuEntry : MonoBehaviour
{

    public Image ButtonImage;
    private TextMeshProUGUI text;
    private Button button;
    private Tooltip tooltip;

    protected void Awake()
    {
        if (ButtonImage == null)
            Debug.LogError($"{name} button image not assigned!");

        text = GetComponentInChildren<TextMeshProUGUI>();

        if (text == null)
            Debug.LogError($"{name} cannot find text child component!");

        button = GetComponentInChildren<Button>();

        if (button == null)
            Debug.LogError($"{name} cannot find button child component!");

        tooltip = GetComponentInChildren<Tooltip>();

        if (tooltip == null)
            Debug.LogError($"{name} cannot find tooltip child component!");
    }

    public void SetSprite(Sprite sprite)
    {
        ButtonImage.sprite = sprite;
    }

    public void SetAction(UnityAction action)
    {
        button.onClick.AddListener(action);
    }

    public void SetPrice(int price)
    {
        text.text = price.ToString();
    }

    public void SetEnabled(bool enabled)
    {
        button.interactable = enabled;
    }

    public void SetTooltipText(string text)
    {
        tooltip.SetText(text);
    }

}
