using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public abstract class SKUIElement : MonoBehaviour
{


    private float _timerResize = 0.1f;

    private void OnEnable()
    {
        _timerResize = 0.1f;
    }


    bool b = false;

    public virtual void Update()
    {
        if (_timerResize >= 0f)
        {
            _timerResize -= Time.deltaTime;
            b = true;
        }
        else if (b == true)
        {
            AttemptResizeElement();
            b = false;
        }
    }

    [FoldoutGroup("DEBUG")]
    [Button("Test_Resize Element")]
    public abstract void AttemptResizeElement();

}
