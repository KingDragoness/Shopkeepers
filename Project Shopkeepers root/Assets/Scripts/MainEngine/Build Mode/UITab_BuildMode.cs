using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class UITab_BuildMode : MonoBehaviour
{

    public PrimaryUITab tabUI;
    public Button button_Wall;


    private void OnDisable()
    {
        BuildMode.CloseBuildTool();
    }

    private void Update()
    {
        if (BuildMode.CurrentTool == BuildToolType.Wall)
        {
            button_Wall.interactable = false;
        }
        else
        {
            button_Wall.interactable = true;
        }


    }

}
