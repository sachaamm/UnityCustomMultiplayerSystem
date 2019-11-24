using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerFollow : MonoBehaviour
{
    // MOVING VAR
    private Transform myTransform;
    public Transform target;
    public float moveSpeed = 3.0f;
    public float rotationSpeed = 3.0f;
    public float range = 10.0f;
    public float range2 = 10.0f;
    public float stop = 0.0f;

    public bool targetReturned = false;
    private bool targetReached = false;

    public int typeOfMoving = 1;
    // Use this for initialization
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {


        if (target == null)
        {

            targetReturned = false;
            return;
        }

        targetReturned = true;

        Vector3 diff = target.position - transform.position;

        myTransform.position += diff / 15;


        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, target.rotation, 0.15f);



    }



    public void setTarget(Transform t)
    {

        target = t;


    }

    public Transform returnTarget()
    {

        if (target != null)
            return target;

        return null;
    }



    public void setTargetReturned(bool b)
    {

        targetReturned = b;
    }


    public void setTypeOfMoving(int n)
    {

        typeOfMoving = n;

    }

}
