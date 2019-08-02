using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragHandler : MonoBehaviour
{
    //Initialize Variables
    GameObject getTarget;

    public LayerMask mask;

    private void Start()
    {
        
    }

    void Update()
    {

        //Mouse Button Press Down
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            getTarget = ReturnClickedObject(out hitInfo);
            if (getTarget != null)
            {
                getTarget.GetComponent<Pelanggan>().TappedByUser();
            }
        }
        
    }

    //Method to Return Clicked Object
    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit, Mathf.Infinity, mask))
        {
            target = hit.collider.gameObject;
        }

        return target;
    }
}
