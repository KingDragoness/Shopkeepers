using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetCreator_OrbitCamera : MonoBehaviour
{

    public MouseLook mouseLook;
    public Transform cameraTransform;
    public float speedZoom = 15f;

    private float minDist = 1f;
    private float maxDist = 8f;
    private float currentDist = 5f;

    private void Update()
    {
        Vector3 cameraLocalPos = new Vector3(0, 0, -currentDist);

        if (Input.GetMouseButton(1))
        {
            mouseLook.disableInput = false;
        }
        else
        {
            mouseLook.disableInput = true;
        }

        float axis_Scrollwheel = Input.GetAxis("Mouse ScrollWheel") * speedZoom;
        currentDist += axis_Scrollwheel * Time.deltaTime;

        cameraTransform.transform.localPosition = cameraLocalPos;

    }

}
