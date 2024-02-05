using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using SFB;


public class AssetCreatorWindow : MonoBehaviour
{

    [FoldoutGroup("Header")] public Text label_headerFileName;
    [ReadOnly] public GameObject workspace_Parent;
    public Text text_Console;
    public Transform parentContent;

    public string targetFilePath = "";

    public string FileExtension = "png";


    private void Start()
    {
        workspace_Parent = new GameObject();
    }

    private void OnDisable()
    {
        if (workspace_Parent != null) workspace_Parent.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (workspace_Parent != null) workspace_Parent.gameObject.SetActive(true);

    }

    public void Button_NewFile()
    {
        NewFile();
    }
    public void Button_SaveFile()
    {

    }
    public void Button_LoadFile()
    {

    }


    protected virtual void NewFile()
    {

        targetFilePath = "";
        RefreshUI();

    }

    public virtual void SaveFile<T>(object skAsset) where T : SKAsset
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

    public virtual T LoadFile<T>(string extension) where T : SKAsset
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extension, false);

        try
        {

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


    public void RefreshUI()
    {
        label_headerFileName.text = $"{targetFilePath}";
    }

}
