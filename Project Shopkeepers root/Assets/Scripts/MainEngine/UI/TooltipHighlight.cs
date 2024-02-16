﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipHighlight : MonoBehaviour
{

    public Text label_Tooltip;
    public RectTransform rectTransform;
    public Vector2 offset = new Vector2(2f,6f);

    public void ShowTooltip(string content = "")
    {
        gameObject.SetActive(true);
        label_Tooltip.text = content;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
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