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

public class AssetCreatorUI_Material : AssetCreatorWindow
{

    public Shader defaultShader;
    public SK_Material skAsset_Mat;
    public RawImage rawImage;
    [ReadOnly] public Texture2D loadedTextureMain;
    [ReadOnly] public Material customMaterial;
    [FoldoutGroup("Template Object")] public GameObject templateObject;
    [ReadOnly] [FoldoutGroup("Template Object")] public GameObject workspace_TestMaterialGO;
    [ReadOnly] [FoldoutGroup("Template Object")] public MeshRenderer workspace_TestMaterialRrndr;
    public Color albedoColor;
    [ReadOnly] public SKUI_FilepathField inputfield_imagePath;
    [ReadOnly] public SKUI_InputField inputfield_imageName;
    [ReadOnly] public SKUI_Vector3IntField inputfield_color;


    private void Awake()
    {
        InitiateUI();
        workspace_TestMaterialGO = Instantiate(templateObject);
        workspace_TestMaterialGO.gameObject.name = "Test Material";
        workspace_TestMaterialRrndr = workspace_TestMaterialGO.GetComponent<MeshRenderer>();
        customMaterial = new Material(defaultShader);
    }

    public override void Start()
    {
        base.Start();
        workspace_Parent.name = $"{this.GetType().ToString()}";
        workspace_TestMaterialGO.transform.SetParent(workspace_Parent.transform);
        workspace_TestMaterialGO.gameObject.SetActive(true);
        workspace_TestMaterialRrndr.material = customMaterial;
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
        SaveFile<SK_Material>(skAsset_Mat, "skmat");
        RefreshMat();

    }

    public override void Button_LoadFile()
    {
        var newSKAsset = LoadFile<SK_Material>(FileExtension);
        if (newSKAsset == null) return;
        skAsset_Mat = newSKAsset;
        inputfield_imageName.inputfield.text = skAsset_Mat.fileName;
        inputfield_color.field_x.text = skAsset_Mat.AlbedoColor.x.ToString();
        inputfield_color.field_y.text = skAsset_Mat.AlbedoColor.y.ToString();
        inputfield_color.field_z.text = skAsset_Mat.AlbedoColor.z.ToString();

        RefreshMat();
        RefreshUI();
    }

    public override void NewFile_PromptConfirm(string[] args)
    {
        skAsset_Mat = new SK_Material();
        inputfield_imageName.inputfield.text = "";
        inputfield_color.field_x.text = "";
        inputfield_color.field_y.text = "";
        inputfield_color.field_z.text = "";
        base.NewFile_PromptConfirm(args);
        RefreshMat();
    }

    public override void RefreshUI()
    {
        skAsset_Mat.fileName = inputfield_imageName.inputfield.text;
        base.RefreshUI();
        string s = "";

        s += $"{skAsset_Mat.fileName}{skAsset_Mat.Extension}\n";
        {
            string allPaths = "";

            allPaths += $"> texture file filepath: {skAsset_Mat.filePath_MainTexture}\n";
            foreach (var path in skAsset_Mat.filePath_MainTexture)
            {
                
            }

            s += allPaths;
        }

        {
            int x = 0;
            int y = 0;
            int z = 0;

            if (int.TryParse(inputfield_color.field_x.text, out x))
            {
                if (x < 0)
                {
                    x = 0;
                }
                if (x > 255)
                {
                    x = 255;
                }

                inputfield_color.field_x.text = x.ToString();
                skAsset_Mat.AlbedoColor.x = x;
            }
            if (int.TryParse(inputfield_color.field_y.text, out y))
            {
                if (y < 0)
                {
                    y = 0;
                }
                if (y > 255)
                {
                    y = 255;
                }

                inputfield_color.field_y.text = y.ToString();
                skAsset_Mat.AlbedoColor.y = y;
            }
            if (int.TryParse(inputfield_color.field_z.text, out z))
            {
                if (z < 0)
                {
                    z = 0;
                }
                if (z > 255)
                {
                    z = 255;
                }

                inputfield_color.field_z.text = z.ToString();
                skAsset_Mat.AlbedoColor.z = z;

            }

            s += $"({x}, {y}, {z})\n";
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
            skAsset_Mat.filePath_MainTexture = Shopkeeper.Database.ProcessUriPath(uri.LocalPath);
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

        inputfield_color = Shopkeeper.UI.DrawVector3IntField(parentContent, "r", "g", "b");
        inputfield_color.field_x.onEndEdit.AddListener(OnEndEdit_InputField);
        inputfield_color.field_y.onEndEdit.AddListener(OnEndEdit_InputField);
        inputfield_color.field_z.onEndEdit.AddListener(OnEndEdit_InputField);

        inputfield_color.field_x.text = "255";
        inputfield_color.field_y.text = "255";
        inputfield_color.field_z.text = "255";

    }


    public void OnEndEdit_InputField(string s)
    {
        RefreshUI();
        RefreshColor();
    }

    private void RefreshMat()
    {
        if (loadedTextureMain != null)
        {
            Destroy(loadedTextureMain);
        }

        string completepath = Shopkeeper.Path_StreamingAssets + skAsset_Mat.filePath_MainTexture;
        var textureObj = Shopkeeper.Database.LoadTexture(completepath);

        if (textureObj != null)
        {
            loadedTextureMain = textureObj;
        }

        rawImage.texture = loadedTextureMain;
        RefreshColor();
    }

    private void RefreshColor()
    {
        Color albedoColor = new Color(
            (float)skAsset_Mat.AlbedoColor.x / 255f,
            (float)skAsset_Mat.AlbedoColor.y / 255f,
            (float)skAsset_Mat.AlbedoColor.z / 255f);

        rawImage.color = albedoColor;

        customMaterial.SetTexture("_MainTex", loadedTextureMain);
        customMaterial.SetColor("_Color", albedoColor);
    }
 
}
