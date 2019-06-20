using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    public float ZoomMin = 3f;
    public float ZoomMax = 7f;
    public float ZoomSpeed = 0.3f;
    public float Smooth = 0.5f;
    public float CurrentZoom { get; private set; }
    public bool InvertScroll = true;

    private CameraMovement cm;

    protected void Awake()
    {
        cm = GetComponent<CameraMovement>();

        if (cm == null)
            Debug.LogError($"{name}: cannot find camera movement component!");

        ZoomMax = ZoomMax * Screen.height / 1080f;
    }

    protected void Update()
    {
        if (cm.CameraLocked)
            return;

        float currentSize = Camera.main.orthographicSize;
        float scroll = Input.GetAxis("Mouse ScrollWheel") * ((InvertScroll) ? 1 : -1);
        float targetSize = Mathf.Clamp(currentSize + (scroll * ZoomSpeed), ZoomMin, ZoomMax);
        CurrentZoom = targetSize;

        Camera.main.orthographicSize = Mathf.Lerp(currentSize, targetSize, Smooth);
    }

}
