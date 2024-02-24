using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class BuildMode_ArrowIcon : MonoBehaviour
{

    public SpriteRenderer spriteRendr_Icon;
    public SpriteRenderer spriteRendr_Arrow;
    public Color color_ArrowUsed;
    public Color color_ArrowIdle;
    public float maxDistance = 45f;
    public LayerMask layerMask_ray;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, layerMask_ray))
        {
            SetPosition(hit.point);

        }
    }

    public void SetArrowColorUsed()
    {
        spriteRendr_Icon.color = color_ArrowUsed;
        spriteRendr_Arrow.color = color_ArrowUsed;
    }
    public void SetArrowColorIdle()
    {
        spriteRendr_Icon.color = color_ArrowIdle;
        spriteRendr_Arrow.color = color_ArrowIdle;
    }

    private void SetPosition(Vector3 pos)
    {
        Vector3 snapPos = pos;
        snapPos.x = Mathf.Round(pos.x);
        snapPos.y = Mathf.Round(pos.y);
        snapPos.z = Mathf.Round(pos.z);

        if (snapPos.y > Shopkeeper.Game.currentLevel * 4f)
        {
            snapPos.y = Shopkeeper.Game.currentLevel * 4f;
        }

        snapPos.x = Mathf.Clamp(snapPos.x, -Lot.MyLot.lotSize.x/2f, Lot.MyLot.lotSize.x/2f);
        snapPos.z = Mathf.Clamp(snapPos.z, 0f, Lot.MyLot.lotSize.y);

        transform.position = snapPos;
    }

    public Vector3Int GetArrowPosition()
    {
        Vector3Int result = new Vector3Int();

        result.x = Mathf.RoundToInt(transform.position.x);
        result.y = Mathf.RoundToInt(transform.position.y);
        result.z = Mathf.RoundToInt(transform.position.z);

        return result;
    }

}
