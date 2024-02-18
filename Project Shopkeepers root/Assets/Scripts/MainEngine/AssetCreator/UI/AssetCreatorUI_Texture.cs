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


public class AssetCreatorUI_Texture : AssetCreatorWindow
{

    public SK_Texture skAsset_Texture;
    public RawImage rawImage;
    public Texture2D loadedTextureMain;
    [ReadOnly] public SKUI_FilepathField inputfield_imagePath;
    [ReadOnly] public SKUI_InputField inputfield_imageName;


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


    public override void Button_SaveFile()
    {
        SaveFile<SK_Texture>(skAsset_Texture, "sktex");
        RefreshMat();

    }

    public override void Button_LoadFile()
    {
        var newSKAsset = LoadFile<SK_Texture>(FileExtension);
        if (newSKAsset == null) return;
        skAsset_Texture = newSKAsset;
        inputfield_imageName.inputfield.text = skAsset_Texture.fileName;

        RefreshMat();
        RefreshUI();
    }

    public override void NewFile_PromptConfirm(string[] args)
    {
        skAsset_Texture = new SK_Texture();
        inputfield_imageName.inputfield.text = "";
        base.NewFile_PromptConfirm(args);
        RefreshMat();
    }

    public override void RefreshUI()
    {
        base.RefreshUI();
        string s = "";

        s += $"{skAsset_Texture.fileName}{skAsset_Texture.Extension}\n";
        {
            string allPaths = "";

            allPaths += $"> texture file filepath: {skAsset_Texture.filePath_MainTexture}\n";
            foreach (var path in skAsset_Texture.filePath_MainTexture)
            {

            }

            s += allPaths;
        }


        text_Console.text = s;
    }

    #endregion

    //special for location field:
    public void ClickOpen_FieldLocation()
    {
        var extensions = new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
                //new ExtensionFilter("All Files", "*" ),
            };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions: extensions, false);
        if (paths.Length > 0)
        {
            var uri = new System.Uri(paths[0]);
            skAsset_Texture.filePath_MainTexture = Shopkeeper.Database.ProcessUriPath(uri.LocalPath);
            RefreshUI();
            RefreshMat();
        }
    }


    public void InitiateUI()
    {
        text_Console.text = "";

        inputfield_imagePath = Shopkeeper.UI.DrawFileLocationField(parentContent, "Add image texture: ", placeholderText: "Select file location...");
        inputfield_imagePath.button.onClick.AddListener(ClickOpen_FieldLocation);

        inputfield_imageName = Shopkeeper.UI.DrawTextInputField(parentContent, "File name: ", placeholderText: "Enter name...");
        inputfield_imageName.inputfield.onEndEdit.AddListener(OnEndEdit_InputField);

        //var dropdown1 = Shopkeeper.UI.DrawDropdown(parentContent, new string[2] { "Standard (Specular)", "Diffuse Normal" }, "Shader Type");
    }


    public void OnEndEdit_InputField(string s)
    {
        skAsset_Texture.fileName = s;
        RefreshUI();
    }

    private void RefreshMat()
    {
        if (loadedTextureMain != null)
        {
            Destroy(loadedTextureMain);
        }

        string completepath = Shopkeeper.Path_StreamingAssets + skAsset_Texture.filePath_MainTexture;
        var textureObj = Shopkeeper.Database.LoadTexture(completepath);

        if (textureObj != null)
        {
            loadedTextureMain = textureObj;
        }

        rawImage.texture = loadedTextureMain;

    }
}
