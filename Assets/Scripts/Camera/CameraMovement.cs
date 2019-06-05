using UnityEngine;
using EventCallbacks;

public class CameraMovement : MonoBehaviour
{

    public GameObject NEBound;
    public GameObject SWBound;

    public float PanSpeed = 3f;
    [Range(0, 1)]
    public float Smooth = 0.5f;
    public int PixelsPerUnit = 64;

    public bool CameraLocked { get; private set; }

    protected void Awake()
    {
        if (NEBound == null || SWBound == null)
            Debug.LogError("Bounds not set!");

        CameraLocked = true;

        // Register listeners
        GameStartEvent.RegisterListener(GameStart);
        GameWonEvent.RegisterListener(GameWon);
        GameOverEvent.RegisterListener(GameOver);
    }

    private void GameStart(GameStartEvent e)
    {
        CameraLocked = false;
    }

    private void GameWon(GameWonEvent e)
    {
        CameraLocked = true;
    }

    private void GameOver(GameOverEvent e)
    {
        CameraLocked = true;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (CameraLocked)
            return;

        // Get current position in pixel perfect coordinates
        Vector3 currPosPP = CalcPixelPerfectPos(transform.position);
        Vector2 currPos = new Vector2(currPosPP.x, currPosPP.y);

        // Calculate target position
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input.Scale(new Vector2(PanSpeed * Time.deltaTime, PanSpeed * Time.deltaTime));
        input = input + currPos;

        // Smooth our movement
        Vector2 moveTo = Vector2.Lerp(currPos, input, Smooth);

        // Clamp our desired position to the specified bounds
        float screenHeight = Camera.main.orthographicSize;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        Vector3 theoreticalPos = new Vector3(Mathf.Clamp(moveTo.x, SWBound.transform.position.x + screenWidth, NEBound.transform.position.x - screenWidth),
                                             Mathf.Clamp(moveTo.y, SWBound.transform.position.y + screenHeight, NEBound.transform.position.y - screenHeight),
                                             transform.position.z);

        // Recalculate pixel perfect coordinates
        Vector3 pixelPerfectPos = CalcPixelPerfectPos(theoreticalPos);

        // Move
        transform.position = pixelPerfectPos;
    }

    private Vector3 CalcPixelPerfectPos(Vector3 theoreticalPos)
    {
        // Scale by PPU
        Vector3 pixelPerfectPos = new Vector3(Mathf.RoundToInt(theoreticalPos.x * PixelsPerUnit),
                                              Mathf.RoundToInt(theoreticalPos.y * PixelsPerUnit),
                                              theoreticalPos.z);

        // Scale by inverse PPU
        pixelPerfectPos.Scale(new Vector3(1f / PixelsPerUnit, 1f / PixelsPerUnit, 1f));
        return pixelPerfectPos;
    }

}
