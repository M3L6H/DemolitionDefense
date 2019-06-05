using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderMenu : MonoBehaviour
{

    public float Smoothness = 0.5f;
    private bool onScreen;
    public RectTransform Rect { get; private set; }
    private float defaultY;
    private float targetY;

    protected void Start()
    {
        Rect = GetComponent<RectTransform>();
        defaultY = Rect.localPosition.y;
        targetY = Rect.localPosition.y - Rect.sizeDelta.y;

        if (Rect == null)
            Debug.LogError($"{name}: cannot find its rect transform!");

        onScreen = false;
    }

    protected void Update()
    {
        Vector3 pos = Rect.localPosition;
        pos.y = onScreen ? targetY : defaultY;
        Rect.localPosition = Vector3.Lerp(Rect.localPosition, pos, Smoothness);
    }

    public void ToggleMenu(bool toggle)
    {
        onScreen = toggle;
    }

}
