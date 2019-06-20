using TMPro;
using UnityEngine;

public class PlacingTower : MonoBehaviour
{

    private TextMeshProUGUI textComponent;
    private bool visible = true;

    protected void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        if (textComponent == null)
            Debug.LogError($"{name}: no text component found!");
    }

    protected void Update()
    {
        if (visible)
        {
            if (textComponent.color.a < 0.05f)
                visible = false;

            Color c = new Color(textComponent.color.r,
                                textComponent.color.g,
                                textComponent.color.b,
                                Mathf.Lerp(textComponent.color.a, 0f, Time.deltaTime));

            textComponent.color = c;
        } else
        {
            if (textComponent.color.a > 0.95f)
                visible = true;

            Color c = new Color(textComponent.color.r,
                                textComponent.color.g,
                                textComponent.color.b,
                                Mathf.Lerp(textComponent.color.a, 1f, Time.deltaTime));

            textComponent.color = c;
        }
    }

}
