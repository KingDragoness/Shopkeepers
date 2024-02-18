using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LookAtPlayer : MonoBehaviour
{

    public bool useSlowlyRotate = false;
    [ShowIf("useSlowlyRotate", true)]  public float speedRotate = 10;

    public bool billboardCamera = false;
    public bool onlyY = false;
    public Transform targetLook;

    private void Start()
    {
        if(billboardCamera)
        {
            targetLook = Camera.main.transform;
        }
    }

    public void Update()
    {
        if (targetLook == null) return;

        if (useSlowlyRotate == false)
        {
            Vector3 target = targetLook.transform.position;
            if (onlyY)
            {
                target.y = transform.position.y;
            }
            transform.LookAt(target);
        }
        else
        {
            Vector3 target = targetLook.position;
            if (onlyY)
            {
                target.y = transform.position.y;
            }
            var q = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speedRotate * Time.deltaTime);
        }
    }

    [ContextMenu("Look At Target")]
    public void LookNow()
    {
        transform.LookAt(targetLook);

    }

}
