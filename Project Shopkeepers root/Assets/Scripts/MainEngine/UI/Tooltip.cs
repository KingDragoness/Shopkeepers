using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{

    public Text label_Tooltip;
    public RectTransform rectTransform;
    public Vector2 offset = new Vector2(2f,6f);

    private ModularUI_TooltipHighlightable previousHighlightTooltip;


    public static void ShowTooltip(string content = "", ModularUI_TooltipHighlightable highlightScript = null)
    {
        Shopkeeper.UI.Tooltip.ShowTooltip_1(content, highlightScript);
    }


    private void ShowTooltip_1(string content = "", ModularUI_TooltipHighlightable highlightScript = null)
    {
        gameObject.SetActive(true);
        previousHighlightTooltip = highlightScript;
        label_Tooltip.text = content;
    }

    public static void HideTooltip()
    {
        Shopkeeper.UI.Tooltip.gameObject.SetActive(false);
    }

    public static void TryDisable(ModularUI_TooltipHighlightable highlightScript)
    {
        if (Shopkeeper.UI.Tooltip.previousHighlightTooltip == highlightScript)
        {
            HideTooltip();
            Shopkeeper.UI.Tooltip.previousHighlightTooltip = null;
        }
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 posTarget = new Vector2(mousePos.x, mousePos.y) + offset;
        posTarget.x = Mathf.Floor(posTarget.x);
        posTarget.y = Mathf.Floor(posTarget.y - rectTransform.sizeDelta.y);

        if (posTarget.x < 0f) posTarget.x = 0f;
        if (posTarget.y < 0f) posTarget.y = 0f;
        if (posTarget.x > Screen.width - rectTransform.sizeDelta.x) posTarget.x = Screen.width - rectTransform.sizeDelta.x;
        if (posTarget.y > Screen.height - rectTransform.sizeDelta.y) posTarget.y = Screen.height - rectTransform.sizeDelta.y;

        rectTransform.anchoredPosition = posTarget;


    }

}
