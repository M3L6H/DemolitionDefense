using UnityEngine;

public class TrapIn : MonoBehaviour
{

    public TrapCollider Collider;

    public bool Paired { get; private set; }

    private TrapOut trapOut;
    private TrapCollider trapCollider;

    protected void Awake()
    {
        if (Collider == null || Collider.GetComponent<BoxCollider2D>() == null)
            Debug.LogError($"{name}: No collider assigned!");

        Paired = false;
    }

    protected void Start()
    {
        trapCollider = Instantiate(Collider);
        trapCollider.transform.position = transform.position;
    }

    public void SetOut(TrapOut trapOut)
    {
        this.trapOut = trapOut;
        trapCollider.TrapOut = trapOut;
        Paired = true;
    }

}
