using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Dummiesman; //Load OBJ Model
using SFB; //Load OBJ Model
using UnityEngine.Networking;
using ButtonCommandElement = WindowConfirmPrompt.ButtonCommandElement;


public class AssetCreatorUI_3dModel : MonoBehaviour
{

    public AssetCreatorWindow baseAssetWindow;

    public List<GameObject> allLoadedModels = new List<GameObject>();
    public SK_3dModel skAsset_3d;
    [ReadOnly] public SKUI_FilepathField inputfield_3dModelPath;
    [ReadOnly] public SKUI_InputField inputfield_3dModelName;

    private bool hasInitialized = false;

    private void Awake()
    {
        InitiateUI();
        hasInitialized = true;
    }

    private void Start()
    {
        baseAssetWindow.workspace_Parent.name = $"{this.GetType().ToString()}";
    }


    protected void NewFile()
    {
        List<ButtonCommandElement> allButtonElements = new List<ButtonCommandElement>();
        ButtonCommandElement e1 = new ButtonCommandElement(NewFile_PromptConfirm, null, "New file");
        allButtonElements.Add(e1);
        Shopkeeper.UI.ConfirmPrompt.InitiatePrompt("New File", "Unsaved file will be lost. Confirm new file?", allButtonElements);
    }

    private void NewFile_PromptConfirm(string[] args)
    {
        skAsset_3d = new SK_3dModel();
        inputfield_3dModelName.inputfield.text = "";
        Refresh3dModels();
        RefreshUI();
        Shopkeeper.UI.ConfirmPrompt.gameObject.SetActive(false);
    }


    public void Button_SaveFile()
    {
        baseAssetWindow.SaveFile<SK_3dModel>(skAsset_3d);
        Refresh3dModels();

    }

    public void Button_LoadFile()
    {
        skAsset_3d = baseAssetWindow.LoadFile<SK_3dModel>(baseAssetWindow.FileExtension);
        inputfield_3dModelName.inputfield.text = skAsset_3d.fileName;
        Refresh3dModels();
        RefreshUI();
    }


    public void OnEnable()
    {
        if (hasInitialized == false) return;

        RefreshUI();
        
    }

    public void InitiateUI()
    {
        baseAssetWindow.text_Console.text = "";

        inputfield_3dModelPath = Shopkeeper.UI.DrawFileLocationField(baseAssetWindow.parentContent, "Add 3d model: ", placeholderText: "Select file location...");
        inputfield_3dModelPath.button.onClick.AddListener(ClickOpen_FieldLocation);

        inputfield_3dModelName = Shopkeeper.UI.DrawTextInputField(baseAssetWindow.parentContent, "File name: ", placeholderText: "Enter name...");
        inputfield_3dModelName.inputfield.onEndEdit.AddListener(OnEndEdit_InputField);

    }

    public void OnEndEdit_InputField(string s)
    {
        skAsset_3d.fileName = s;
        RefreshUI();
    }

    //special for location field:
    public void ClickOpen_FieldLocation()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", baseAssetWindow.FileExtension, false);
        if (paths.Length > 0)
        {
            var uri = new System.Uri(paths[0]);
            Shopkeeper.Database.ProcessUriPath(uri.LocalPath);
            RefreshUI();
            Refresh3dModels();
        }
    }


    //refreshing 3d models
    private void Refresh3dModels()
    {
        foreach (var go in allLoadedModels)
        {
            if (go == null) continue;
            Destroy(go.gameObject);
        }
        allLoadedModels.Clear();


        foreach (var path in skAsset_3d.filePath_3dModelPaths)
        {
            string completepath = Shopkeeper.Path_StreamingAssets + path;
            var modelObj = Shopkeeper.Database.Load3dModel(completepath);

            if (modelObj != null)
            {
                allLoadedModels.Add(modelObj);
                modelObj.transform.SetParent(baseAssetWindow.workspace_Parent.transform);
            }
        }
    }

    public void RefreshUI()
    {
        string s = "";

        s += $"{skAsset_3d.fileName}{skAsset_3d.Extension}\n";
        {
            string allPaths = "";

            foreach(var path in skAsset_3d.filePath_3dModelPaths)
            {
                allPaths += $"> .obj filepath: {path}\n";
            }

            s += allPaths;
        }


        baseAssetWindow.text_Console.text = s;
    }

}
