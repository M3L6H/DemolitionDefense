using UnityEngine;

public class TrapOut : MonoBehaviour
{

    public bool Paired { get; private set; }

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

}
