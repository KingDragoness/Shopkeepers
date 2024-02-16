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


public class AssetCreatorUI_3dModel : AssetCreatorWindow
{

    public List<GameObject> allLoadedModels = new List<GameObject>();
    public SK_3dModel skAsset_3d;
    [ReadOnly] public SKUI_FilepathField inputfield_3dModelPath;
    [ReadOnly] public SKUI_InputField inputfield_3dModelName;


    private void Awake()
    {
        InitiateUI();
    }

    public override void Start()
    {
        base.Start();
        workspace_Parent.name = $"{this.GetType().ToString()}";
    }

    #region Parent Overrides
    public override void OnEnable()
    {
        base.OnEnable();
        Shopkeeper.AssetCreator.EnableAssetCreator();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Shopkeeper.AssetCreator.DisableAssetCreator();
    }

    public override void Button_SaveFile()
    {
        SaveFile<SK_3dModel>(skAsset_3d);
        Refresh3dModels();

    }

    public override void Button_LoadFile()
    {
        var newSKAsset = LoadFile<SK_3dModel>(FileExtension);
        if (newSKAsset == null) return;
        skAsset_3d = newSKAsset;
        inputfield_3dModelName.inputfield.text = skAsset_3d.fileName;
        Refresh3dModels();
        RefreshUI();
    }

    public override void NewFile_PromptConfirm(string[] args)
    {
        skAsset_3d = new SK_3dModel();
        inputfield_3dModelName.inputfield.text = "";
        base.NewFile_PromptConfirm(args);
        Refresh3dModels();
    }

    public override void RefreshUI()
    {
        skAsset_3d.fileName = inputfield_3dModelName.inputfield.text;

        base.RefreshUI();
        string s = "";

        s += $"{skAsset_3d.fileName}{skAsset_3d.Extension}\n";
        {
            string allPaths = "";

            foreach (var path in skAsset_3d.filePath_3dModelPaths)
            {
                allPaths += $"> .obj filepath: {path}\n";
            }

            s += allPaths;
        }


        text_Console.text = s;
    }

    #endregion



    public void InitiateUI()
    {
        text_Console.text = "";

        inputfield_3dModelPath = Shopkeeper.UI.DrawFileLocationField(parentContent, "Add 3d model: ", placeholderText: "Select file location...");
        inputfield_3dModelPath.button.onClick.AddListener(ClickOpen_FieldLocation);

        inputfield_3dModelName = Shopkeeper.UI.DrawTextInputField(parentContent, "File name: ", placeholderText: "Enter name...");
        inputfield_3dModelName.inputfield.onEndEdit.AddListener(OnEndEdit_InputField);

        Shopkeeper.UI.DrawButtonField(parentContent, "COME ON", 48f);
    }

    public void OnEndEdit_InputField(string s)
    {
        RefreshUI();
    }

    //special for location field:
    public void ClickOpen_FieldLocation()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false);
        if (paths.Length > 0)
        {
            var uri = new System.Uri(paths[0]);
            skAsset_3d.filePath_3dModelPaths.Add(Shopkeeper.Database.ProcessUriPath(uri.LocalPath));
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
                modelObj.transform.SetParent(workspace_Parent.transform);
            }
        }
    }


}
