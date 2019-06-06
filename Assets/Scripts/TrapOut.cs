using UnityEngine;

public class TrapOut : MonoBehaviour
{

    public bool Paired;

    private TrapIn trapIn;

    protected void Awake()
    {
        Paired = false;
    }

    protected void Start()
    {
        TrapIn[] traps = FindObjectsOfType<TrapIn>();

        foreach (TrapIn trap in traps)
        {
            if (!trap.Paired)
            {
                trapIn = trap;
                Paired = true;
                trapIn.SetOut(this);
                break;
            }
        }
    }

    public void SetIn(TrapIn trapIn)
    {
        this.trapIn = trapIn;
        Paired = true;
    }

    protected void OnDestroy()
    {
        trapIn.Paired = false;
    }

}
