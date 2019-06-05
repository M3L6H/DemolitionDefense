using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goals : MonoBehaviour
{

    public Goal[] GetGoals()
    {
        return GetComponentsInChildren<Goal>();
    }

}
