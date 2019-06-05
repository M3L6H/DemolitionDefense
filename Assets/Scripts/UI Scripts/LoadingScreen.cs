using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour
{

    [Range(0.1f, 1f)]
    public float Speed = 0.2f;
    private float elapsedTime = 0f;
    private int character = 0;

    private TextMeshProUGUI text;
    private string textContents;

    protected void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();

        if (text == null)
            Debug.LogError($"{name}: unable to find text component in child!");
    }

    protected void Start()
    {
        gameObject.SetActive(false);
        textContents = text.text;
        text.text = "";
    }

    protected void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= Speed)
        {
            elapsedTime = 0f;
            text.text = textContents.Substring(0, character + 1);
            character = (character + 1) % textContents.Length;
        }
    }

}
