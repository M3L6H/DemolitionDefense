using UnityEngine;

public class Goals : MonoBehaviour
{

    public Goal[] GetGoals()
    {
        return GetComponentsInChildren<Goal>();
    }

}
