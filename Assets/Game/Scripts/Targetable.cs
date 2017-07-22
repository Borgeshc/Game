using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public void SetTarget()
    {
        print(gameObject.name + " is targeted");
    }

    public void UnTarget()
    {
        print(gameObject.name + " is not targeted");
    }
}
