using UnityEngine;

public class PusherArm : MonoBehaviour
{

    public float Speed = 1f;

    private float threshold;
    private bool animating = false;
    private bool extended = false;
    private Vector2 targetLoc;
    private Vector2 originalLoc;
    private GameManager gm;

    protected void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        if (gm == null)
            Debug.LogError($"{name}: could not find game manager!");

        threshold = 0.01f * Mathf.Sqrt(Speed);
    }

    protected void Update()
    {
        if (!animating || gm.Paused)
            return;

        if (!extended)
        {
            transform.position += ((Vector3)targetLoc - transform.position).normalized * Speed * Time.deltaTime * gm.GameSpeed;
            if (Vector2.Distance(transform.position, targetLoc) < threshold * gm.GameSpeed)
                extended = true;
        } else
        {
            transform.position += ((Vector3)originalLoc - transform.position).normalized * Speed * Time.deltaTime * gm.GameSpeed;
            if (Vector2.Distance(transform.position, originalLoc) < threshold * gm.GameSpeed)
            {
                extended = false;
                animating = false;
            }
        }
    }

    public void Animate(Vector2 direction)
    {
        originalLoc = transform.position;
        targetLoc = originalLoc + direction * 0.8f;
        animating = true;
    }

}
