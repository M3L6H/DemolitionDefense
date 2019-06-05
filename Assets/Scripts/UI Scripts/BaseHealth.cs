using UnityEngine;
using TMPro;
using EventCallbacks;

public class BaseHealth : MonoBehaviour
{

    private TextMeshProUGUI textBox;
    private GameManager gm;

    protected void Awake()
    {
        textBox = GetComponent<TextMeshProUGUI>();

        if (textBox == null)
            Debug.LogError($"{name} does not have a text component!");

        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name} no game manager found!");

        // Register listeners
        BaseDamageUIEvent.RegisterListener(BaseDamaged);
    }

    protected void Start()
    {
        textBox.text = gm.BaseHealth.ToString();
    }

    private void BaseDamaged(BaseDamageUIEvent e)
    {
        textBox.text = gm.BaseHealth.ToString();
    }

}
