using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class RectTextCalculateTest : MonoBehaviour
{
    
    [Button("TEST")]
    public void Test()
    {
        var width1 = GetComponent<Text>().preferredWidth;
        Debug.Log(width1);
    }

}
