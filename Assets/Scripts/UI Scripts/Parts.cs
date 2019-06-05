using UnityEngine;
using TMPro;
using EventCallbacks;

public class Parts : MonoBehaviour
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
        PartsChangedUIEvent.RegisterListener(PartsChanged);
    }

    protected void Start()
    {
        textBox.text = gm.Parts.ToString();
    }

    private void PartsChanged(PartsChangedUIEvent e)
    {
        textBox.text = gm.Parts.ToString();
    }

}
