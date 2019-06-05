using UnityEngine;

public class Arrow : MonoBehaviour
{

    public SpriteRenderer MySpriteRenderer;

    [HideInInspector]
    public bool Flipped = false;

    [HideInInspector]
    public Vector2 ArrowDirection = Vector2.up;

    [HideInInspector]
    public Type ArrowType = Type.Straight;

    private Sprite[] texture;

    public enum Type
    {
        Straight,
        Curved,
        HorizontalMerge,
        VerticalMerge,
        ThreeWayMerge
    }

    protected void Awake()
    {
        if (MySpriteRenderer == null)
            Debug.LogError("No sprite renderer assigned!");
    }

    protected void Update()
    {
        
    }

    public void UpdateGraphics(Sprite[] graphics)
    {
        MySpriteRenderer.sprite = graphics[0];
        MySpriteRenderer.flipX = Flipped && (ArrowDirection == Vector2.left || ArrowDirection == Vector2.right);
        MySpriteRenderer.flipY = Flipped && (ArrowDirection == Vector2.up || ArrowDirection == Vector2.down);
        //Rotation((Flipped) ? -ArrowDirection : ArrowDirection);
        Rotation(ArrowDirection);
        texture = graphics;
    }

    private void Rotation(Vector2 direction)
    {
        if (direction == Vector2.right)
            MySpriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (direction == Vector2.up)
            MySpriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        else if (direction == Vector2.left)
            MySpriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        else
            MySpriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
    }

}
