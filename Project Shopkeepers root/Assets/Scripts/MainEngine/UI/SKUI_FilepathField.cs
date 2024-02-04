using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SKUI_FilepathField : SKUIElement
{

    public Text label;
    public InputField inputField;
    public Button button;

    public override void AttemptResizeElement()
    {
        float[] sizes = new float[3];
        sizes[0] = label.preferredWidth;
        sizes[2] = button.GetComponent<RectTransform>().sizeDelta.x;
        sizes[1] = GetComponent<RectTransform>().sizeDelta.x - sizes[2] - sizes[0];

        var size1 = label.GetComponent<RectTransform>().sizeDelta;

        label.GetComponent<RectTransform>().sizeDelta = new Vector2(sizes[0], size1.y);

        var rt1 = inputField.GetComponent<RectTransform>();
        var posrt1 = new Vector2();
        posrt1.x = sizes[0];
        rt1.anchoredPosition = posrt1;
        rt1.sizeDelta = new Vector2(sizes[1], rt1.sizeDelta.y);
    }

}
