using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;

public class CameraShake : MonoBehaviour
{

    [Range(0.1f, 1)]
    public float Intensity;

    [Range(0.5f, 3f)]
    public float Duration;

    private Vector3 initialPos;
    private bool isShaking = false;
    private float timeElapsed;

    private readonly Vector3[] offsets =
    {
        new Vector3(1f, 1f, 0f),
        new Vector3(1f, -1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3(-1f, -1f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(-1f, 0f, 0f),
        new Vector3(0f, -1f, 0f)
    };

    protected void Awake()
    {
        BaseDamageEvent.RegisterListener(ShakeCamera);
    }

    private void ShakeCamera(BaseDamageEvent e)
    {
        if (isShaking)
            return;

        initialPos = transform.position;
        timeElapsed = 0f;
        isShaking = true;

        StartCoroutine(ShakeCameraCoroutine());
    }

    private IEnumerator ShakeCameraCoroutine()
    {
        Vector3 offset = offsets[Random.Range(0, 7)].normalized * Intensity;
        transform.position = initialPos + offset;
        timeElapsed += 0.1f;

        if (timeElapsed <= Duration)
            yield return new WaitForSecondsRealtime(0.1f);

        isShaking = false;
        transform.position = initialPos;
    }

}
