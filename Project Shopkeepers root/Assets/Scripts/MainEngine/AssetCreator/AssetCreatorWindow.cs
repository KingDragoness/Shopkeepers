using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using SFB;
using ButtonCommandElement = WindowConfirmPrompt.ButtonCommandElement;


public abstract class AssetCreatorWindow : MonoBehaviour
{

    [FoldoutGroup("Header")] public Text label_headerFileName;
    [FoldoutGroup("Header")] public Button button_NewFile;
    [FoldoutGroup("Header")] public Button button_LoadFile;
    [FoldoutGroup("Header")] public Button button_SaveFile;

    [ReadOnly] public GameObject workspace_Parent;
    public Text text_Console;
    public Transform parentContent;

    public string targetFilePath = "";

    public string FileExtension = "png";
    protected bool hasInitialized = false;


    private void Awake()
    {
        hasInitialized = true;
    }

    public virtual void Start()
    {
        workspace_Parent = new GameObject();
        button_NewFile.onClick.AddListener(Button_NewFile);
        button_SaveFile.onClick.AddListener(Button_SaveFile);
        button_LoadFile.onClick.AddListener(Button_LoadFile);

    }

    public virtual void OnDisable()
    {
        if (workspace_Parent != null) workspace_Parent.gameObject.SetActive(false);
    }

    public virtual void OnEnable()
    {
        if (workspace_Parent != null) workspace_Parent.gameObject.SetActive(true);
        if (hasInitialized == false) return;

        RefreshUI();
    }

    public void Button_NewFile()
    {
        List<ButtonCommandElement> allButtonElements = new List<ButtonCommandElement>();
        ButtonCommandElement e1 = new ButtonCommandElement(NewFile_PromptConfirm, null, "New file");
        allButtonElements.Add(e1);
        Shopkeeper.UI.ConfirmPrompt.InitiatePrompt("New File", "Unsaved file will be lost. Confirm new file?", allButtonElements);
    }

    public abstract void Button_SaveFile();

    public abstract void Button_LoadFile();


    public virtual void NewFile_PromptConfirm(string[] args)
    {
        Shopkeeper.UI.ConfirmPrompt.gameObject.SetActive(false);
        targetFilePath = "";
        RefreshUI();
    }


    public virtual void SaveFile<T>(object skAsset, string extension = "sk3d") where T : SKAsset
    {
        T skAsset_derivative = skAsset as T;
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", skAsset_derivative.fileName, extension);
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


    public virtual void RefreshUI()
    {
        label_headerFileName.text = $"{targetFilePath}";
    }

}
