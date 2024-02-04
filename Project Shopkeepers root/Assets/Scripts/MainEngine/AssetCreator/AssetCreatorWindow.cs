using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using SFB;


public abstract class AssetCreatorWindow : MonoBehaviour
{

    [FoldoutGroup("Header")] public Text label_headerFileName;

    public string targetFilePath = "";

    //for stupid button
    public virtual void Button_NewFile()
    {
        NewFile();
    }
    public abstract void Button_SaveFile();
    public abstract void Button_LoadFile();


    protected virtual void NewFile()
    {

        targetFilePath = "";
        RefreshUI();

    }

    protected virtual void SaveFile<T>(object skAsset) where T : SKAsset
    {
        T skAsset_derivative = skAsset as T;
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", skAsset_derivative.fileName, "sk3d");
        string content = Shopkeeper.AssetCreator.ConvertSKAssetToJSON<SKAsset>(skAsset_derivative);
        if (!string.IsNullOrEmpty(path))
        {
            Shopkeeper.Database.SaveAssetFile(path, "", content);
        }

        targetFilePath = path;

        RefreshUI();
    }

    protected virtual T LoadFile<T>(string extension) where T : SKAsset
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extension, false);

        try
        {
            NewFile();

            targetFilePath = path[0];

            var skAsset_d = Shopkeeper.Database.LoadAssetFile<T>(targetFilePath, "");
            RefreshUI();

            return skAsset_d;

        }
        catch
        {
            Debug.LogError($"ERROR! Cannot open file!");
        }

        return null;
    }


    public virtual void RefreshUI()
    {
        label_headerFileName.text = $"{targetFilePath}";
    }

}
