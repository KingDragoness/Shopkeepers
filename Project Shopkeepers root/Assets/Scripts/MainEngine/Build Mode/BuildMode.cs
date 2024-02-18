using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public abstract class BuildToolScript : MonoBehaviour
{
    public abstract void RunTool();
}

public enum BuildToolType
{
    None,
    Wall,
    Flooring,
    Wallpaper,
    DoorWindows
}

public class BuildMode : MonoBehaviour
{

    [SerializeField] private BuildToolType currentTool;
    public BuildToolScript currentToolScript;
    [SerializeField] private BuildMode_ArrowIcon prefab_Arrow;
    [SerializeField] private BuildMode_ArrowIcon currentArrow;
    [SerializeField] private BuildMode_Wall _wall;

    public static BuildMode_Wall Wall
    {
        get { return Shopkeeper.BuildMode._wall; }
    }

    public static BuildToolType CurrentTool { get => Shopkeeper.BuildMode.currentTool; set => Shopkeeper.BuildMode.currentTool = value; }
    public BuildMode_ArrowIcon Arrow { get => currentArrow; }

    public void OpenBuildTool_1(int type)
    {
        OpenBuildTool(type);
    }

    public static void OpenBuildTool(int type)
    {
        Shopkeeper.BuildMode.currentTool = (BuildToolType)type;
        Shopkeeper.BuildMode.ActivateTool();
        Shopkeeper.BuildMode.Refresh();
    }

    public static void CloseBuildTool()
    {
        OpenBuildTool(0);

        if (Shopkeeper.BuildMode.currentArrow != null)
        {
            Destroy(Shopkeeper.BuildMode.currentArrow.gameObject);
        }
    }

    private void ActivateTool()
    {
        if (currentArrow != null)
        {
            Destroy(currentArrow.gameObject);
        }

        if (CurrentTool != BuildToolType.None)
        {
            currentArrow = Instantiate(prefab_Arrow, transform);
            currentArrow.gameObject.SetActive(true);
        }

        if (CurrentTool == BuildToolType.Wall)
        {     
            currentArrow.spriteRendr_Icon.sprite = Shopkeeper.InternalAsset.Icon_Buildmode_Wall;
        }
    }

    private void Refresh()
    {

    }

    private void Update()
    {
        if (CurrentTool == BuildToolType.Wall)
        {
            currentToolScript = _wall;
        }

        if (CurrentTool == BuildToolType.None)
        {
            currentToolScript = null;
        }

        if (currentToolScript != null)
        {
            currentToolScript.RunTool();
        }
    }

    
}
