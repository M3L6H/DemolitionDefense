using UnityEngine;

public class TrapCollider : MonoBehaviour
{

    public TrapOut MyTrapOut { private get; set; }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (MyTrapOut == null)
            Debug.LogError($"{name}: no trap out assigned!");

        // Enemies should not recycle each other
        Movement movement = collision.gameObject.GetComponent<Movement>();
        if (movement != null)
        {
            movement.transform.position = MyTrapOut.transform.position - new Vector3(.5f, .5f, 0f);
            movement.Moved();
        }
    }

}
