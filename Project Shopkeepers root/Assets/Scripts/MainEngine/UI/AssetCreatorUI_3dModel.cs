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


public class AssetCreatorUI_3dModel : AssetCreatorWindow
{

    public List<GameObject> allLoadedModels = new List<GameObject>();
    public SK_3dModel skAsset_3d;
    public Text text_3dModelAsset;
    [ReadOnly] public SKUI_FilepathField inputfield_3dModelPath;
    [ReadOnly] public SKUI_InputField inputfield_3dModelName;
    public Transform parentContent;
    public string filepath_objmtl = "X:/something...";

    public string fieldLocation_Extension = ".jpg";

    private bool hasInitialized = false;

    private void Awake()
    {
        InitiateUI();
        hasInitialized = true;
    }


    protected override void NewFile()
    {
        base.NewFile();
        skAsset_3d = new SK_3dModel();
        Refresh3dModels();
    }


    public override void Button_SaveFile()
    {
        SaveFile<SK_3dModel>(skAsset_3d);
        Refresh3dModels();
    }

    public override void Button_LoadFile()
    {
        skAsset_3d = LoadFile<SK_3dModel>(".sk3d");
    }


    private void OnEnable()
    {
        if (hasInitialized == false) return;

        RefreshUI();
    }

    private void InitiateUI()
    {
        text_3dModelAsset.text = "";

        inputfield_3dModelPath = Shopkeeper.UI.DrawFileLocationField(parentContent, "Add 3d model: ", placeholderText: "Select file location...");
        inputfield_3dModelPath.button.onClick.AddListener(ClickOpen_FieldLocation);

        inputfield_3dModelName = Shopkeeper.UI.DrawTextInputField(parentContent, "File name: ", placeholderText: "Enter name...");
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
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", fieldLocation_Extension, false);
        if (paths.Length > 0)
        {
            var uri = new System.Uri(paths[0]);
            processUriPath(uri.LocalPath);
            RefreshUI();
            Refresh3dModels();
        }
    }

    private void processUriPath(string fullPath)
    {
        string localPath = fullPath;
        string streamingPath = Shopkeeper.Path_StreamingAssets;
        streamingPath = streamingPath.Replace("/", @"\");
        bool b_PathRemoved = false;

        if (localPath.Contains(streamingPath))
        {
            localPath = localPath.Remove(0, streamingPath.Length);
            b_PathRemoved = true;
        }

        if (b_PathRemoved == false)
        {
            foreach (var gamePath in Shopkeeper.Database.all_LoadPathTarget)
            {
                string sz1 = gamePath;
                sz1 = sz1.Replace("/", @"\");

                if (localPath.Contains(sz1))
                {
                    localPath = localPath.Remove(0, sz1.Length);
                }
            }
        }

        skAsset_3d.filePath_3dModelPaths.Add(localPath);

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
            }
        }
    }

    public override void RefreshUI()
    {
        string s = "";
        base.RefreshUI();
        s += $"{skAsset_3d.fileName}{skAsset_3d.Extension}\n";
        {
            string allPaths = "";

            foreach(var path in skAsset_3d.filePath_3dModelPaths)
            {
                allPaths += $"> .obj filepath: {path}\n";
            }

            s += allPaths;
        }


        text_3dModelAsset.text = s;
    }

}
