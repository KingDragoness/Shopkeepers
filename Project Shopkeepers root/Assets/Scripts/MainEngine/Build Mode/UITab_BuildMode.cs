using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class UITab_BuildMode : MonoBehaviour
{

    public PrimaryUITab tabUI;
    public Button button_Wall;
    public GameObject tab_MainBuildingTools;
    public GameObject tab_Tool_Wallpaper;
    public GameObject tab_Tool_Flooring;


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

        if (BuildMode.CurrentTool == BuildToolType.Wallpaper)
        {
            tab_MainBuildingTools.EnableGameobject(false);
            tab_Tool_Wallpaper.EnableGameobject(true);
        }
        else
        {
            tab_Tool_Wallpaper.EnableGameobject(false);

        }

        if (BuildMode.CurrentTool == BuildToolType.Flooring)
        {
            tab_MainBuildingTools.EnableGameobject(false);
            tab_Tool_Flooring.EnableGameobject(true);
        }
        else
        {
            tab_Tool_Flooring.EnableGameobject(false);

        }


        if (BuildMode.CurrentTool == BuildToolType.Wall | BuildMode.CurrentTool == BuildToolType.None)
        {
            tab_MainBuildingTools.EnableGameobject(true);
        }

    }

}
