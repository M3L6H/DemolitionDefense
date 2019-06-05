using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    [Range(0f, 1f)]
    public float percentage;
    public float Smooth = 0.5f;
    public float Scale = 0.8f;

    public GameObject fill;

    private CameraZoom cameraMain;
    private SpriteRenderer spriteRenderer;

    protected void Awake()
    {
        if (fill == null)
            Debug.LogError($"{name}: fill child not assigned!");

        cameraMain = FindObjectOfType<CameraZoom>();

        if (cameraMain == null)
            Debug.LogError($"{name}: could not find camera!");

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogError($"{name}: could not find sprite renderer!");
    }

    protected void Update()
    {
        spriteRenderer.enabled = percentage < 1f;
        fill.SetActive(percentage < 1f);

        fill.transform.localScale = new Vector3(Mathf.Clamp01(percentage), 1f, 1f);
        Vector3 targetScale = new Vector3(cameraMain.CurrentZoom / cameraMain.ZoomMin * Scale,
                                           cameraMain.CurrentZoom / cameraMain.ZoomMin * Scale,
                                           1f);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Smooth);
    }

}
