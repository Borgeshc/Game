using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSystem : MonoBehaviour
{
    public LayerMask layerMask;
    public static GameObject target;

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (hit.transform.tag.Equals("Enemy"))
                {
                    target = hit.transform.gameObject;
                    hit.transform.GetComponent<Targetable>().SetTarget();
                }
            }
            
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (target != null)
                {
                    target.GetComponent<Targetable>().UnTarget();
                    target = null;
                }
            }
        }
    }
}
