using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameCamera : MonoBehaviour
{

    public Transform bait_noX;
    public Transform bait_OrbitalPivot;
    public MouseLook mouseLook;
    public float speedPan = 10f;
    public float speedZoom = 10f;
    public float rotateSpeed = 20f;
    public float screenDistX = 48f;
    public float screenDistY = 32f;

    [ReadOnly] [SerializeField] private bool isDisablePanning = false;
    private Vector3 deltaOriginMouse = new Vector3();
    private Vector3 lastMouseMiddleClick = new Vector3();
    private Vector3 lastMouseMiddle_delta = new Vector3();

    private Transform originalParent;

    private void Start()
    {
        originalParent = transform.parent;
        transform.position = Lot.CenterPivot() + (Vector3.forward * -Lot.MyLot.lotSize.y) + Vector3.up * 5f;
    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.Z)) isDisablePanning = !isDisablePanning;
        if (Input.GetMouseButton(2)) isDisablePanning = true;

        if (isDisablePanning == false)
        {
            ScreenPan();
            if (Input.GetMouseButtonDown(1))
            {
                deltaOriginMouse = Input.mousePosition;
            }
            if (Input.GetMouseButton(1)) ManualFastPan();
        }

        UpdateBait();
        Scrollwheel();
        ClampPosition();
        if (Input.GetMouseButtonDown(2))
        {
            bait_OrbitalPivot.transform.position = transform.position + (transform.forward * transform.position.y);
        }
        if (Input.GetMouseButtonUp(2))
        {
            isDisablePanning = false;
        }
        if (Input.GetMouseButton(2))
        {
            transform.parent = bait_OrbitalPivot;
            mouseLook.enabled = true;
            RotateCamera();
        }
        else
        {
            transform.parent = originalParent;
            bait_OrbitalPivot.transform.position = transform.position;
            mouseLook.enabled = false;
        }

    }


    private void Scrollwheel()
    {
        var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        var deltaZoom = transform.forward * mouseScroll * speedZoom;


        if (transform.position.y + deltaZoom.y < Lot.CameraCeilingMin)
        {
        }
        else if (transform.position.y + deltaZoom.y > Lot.CameraCeilingMax)
        {
        }
        else
        {
            transform.position += deltaZoom * 0.5f;
        }

    }


    private void RotateCamera()
    {
        
        transform.rotation = bait_OrbitalPivot.rotation;
    }

    private void UpdateBait()
    {
        {
            Vector3 targetPosition = transform.position + (transform.forward * 5f);
            targetPosition.y = transform.position.y;


            bait_noX.transform.position = transform.position;
            bait_noX.transform.LookAt(targetPosition);
        }

        {
        }
    }

    private void ManualFastPan()
    {
        if (MainUI.GetEventSystemRaycastResults().Count > 0) return; //cancelled if hit any UI

        Vector3 mousePos = Input.mousePosition;
        Vector3 dir = mousePos - deltaOriginMouse;
        float dist = dir.magnitude;

        dir.Normalize();
        Vector3 dirDelta = bait_noX.transform.forward * dir.y;
        dirDelta += bait_noX.transform.right * dir.x;
        dirDelta.Normalize();


        transform.position += dirDelta * speedPan * Mathf.Clamp(dist * 0.01f, 0f, 1f) * Time.deltaTime;
    }

    private void ScreenPan()
    {
        if (MainUI.GetEventSystemRaycastResults().Count > 0) return; //cancelled if hit any UI

        Vector3 mousePos = Input.mousePosition;
        Vector3 delta = new Vector3();
        float distGroundSpeed = Mathf.Clamp(transform.position.y * 0.05f, 0.2f, 4f);

        if (mousePos.x < screenDistX)
        {
            delta -= bait_noX.transform.right;
        }
        else if (mousePos.x > (Screen.width - screenDistX))
        {
            delta += bait_noX.transform.right;
        }

        if (mousePos.y < screenDistY)
        {
            delta -= bait_noX.transform.forward;

        }
        else if (mousePos.y > (Screen.height - screenDistY))
        {
            delta += bait_noX.transform.forward;

        }


        transform.position += delta * speedPan * distGroundSpeed * Time.deltaTime;
    }

    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        float maxY = Lot.MyLot.lotSize.y + 2f;

        if (pos.x > (Lot.MyLot.lotSize.x * Lot.CameraExtraAreaMult))
        {
            pos.x = Lot.MyLot.lotSize.x * Lot.CameraExtraAreaMult;
        }

        if (pos.x < -Lot.MyLot.lotSize.x * Lot.CameraExtraAreaMult)
        {
            pos.x = -Lot.MyLot.lotSize.x * Lot.CameraExtraAreaMult;
        }

        if (pos.z > maxY * Lot.CameraExtraAreaMult)
        {
            pos.z = maxY * Lot.CameraExtraAreaMult;
        }

        if (pos.z < -maxY * Lot.CameraExtraAreaMult)
        {
            pos.z = -maxY * Lot.CameraExtraAreaMult;
        }

        if (pos.y < Lot.CameraCeilingMin)
        {
            pos.y = Lot.CameraCeilingMin;
        }

        if (pos.y > Lot.CameraCeilingMax)
        {
            pos.y = Lot.CameraCeilingMax;
        }


        transform.position = pos;

    }

}
