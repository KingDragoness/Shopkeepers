using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ModularUI_TooltipHighlightable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [TextArea(2,4)]
    public string content = "";

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.ShowTooltip(content);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip();
    }
}
