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
    public AssetCreatorUI_Material UI_Material;
    public AssetCreatorUI_Texture UI_Texture;

    public static void Enable()
    {
        Shopkeeper.UI.assetCreatorUI.gameObject.SetActive(true);
        Shopkeeper.Game.scene.gameObject.SetActive(false);
        Shopkeeper.AssetCreator.EnableAssetCreator();
    }

    public static void Disable()
    {
        Shopkeeper.UI.assetCreatorUI.gameObject.SetActive(false);
        Shopkeeper.Game.scene.gameObject.SetActive(true);
        Shopkeeper.AssetCreator.DisableAssetCreator();
    }

    public static void Toggle()
    {
        if (Shopkeeper.UI.assetCreatorUI.activeSelf)
        {
            Disable();
        }
        else
        {
            Enable();
        }

    }

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

        if (menuType == AssetCreatorType.Material)
        {
            UI_Material.gameObject.EnableGameobject(true);
        }
        else
        {
            UI_Material.gameObject.EnableGameobject(false);
        }

        if (menuType == AssetCreatorType.Texture)
        {
            UI_Texture.gameObject.EnableGameobject(true);
        }
        else
        {
            UI_Texture.gameObject.EnableGameobject(false);
        }
    }
}




