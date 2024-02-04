using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public enum AssetCreatorType
{
    None,
    Model3d,
    Material,
    Item,
    Category,
    Texture
}

public class AssetCreatorUI : MonoBehaviour
{
    


    public AssetCreatorType menuType;
    public AssetCreatorUI_3dModel UI_3dModel;

    public void OpenUI(int type)
    {
        menuType = (AssetCreatorType)type;
    }

    private void Update()
    {
        if (menuType == AssetCreatorType.Model3d)
        {
            UI_3dModel.gameObject.EnableGameobject(true);

        }
        else
        {
            UI_3dModel.gameObject.EnableGameobject(false);

        }
    }



}
