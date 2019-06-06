using UnityEngine;

public class Pusher : MonoBehaviour
{

    public CardinalDirection MyDirection;

    private PusherArm arm;

    protected void Awake()
    {
        arm = GetComponentInChildren<PusherArm>();

        if (arm == null)
            Debug.LogError($"{name}: could not find pusher arm!");
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Enemies should not recycle each other
        Movement movement = collision.gameObject.GetComponent<Movement>();
        if (movement != null && !movement.Heavy)
        {
            Vector2 direction;
            switch (MyDirection)
            {
                case CardinalDirection.North:
                    direction = Vector2.up;
                    break;
                case CardinalDirection.South:
                    direction = Vector2.down;
                    break;
                case CardinalDirection.East:
                    direction = Vector2.right;
                    break;
                case CardinalDirection.West:
                    direction = Vector2.left;
                    break;
                default:
                    direction = Vector2.zero;
                    break;
            }
            arm.Animate(direction);
            movement.MoveTo(movement.transform.position + (Vector3)direction, arm.Speed);
        }
    }

}
