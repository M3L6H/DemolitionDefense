using UnityEngine;

public class Recycler : MonoBehaviour
{

    public GameObject Collider;

    protected void Awake()
    {
        if (Collider == null || Collider.GetComponent<BoxCollider2D>() == null)
            Debug.LogError($"{name}: No collider assigned!");
    }

    protected void Start()
    {
        GameObject colliderObj = Instantiate(Collider, transform);
        colliderObj.transform.position = transform.position;
    }

}
