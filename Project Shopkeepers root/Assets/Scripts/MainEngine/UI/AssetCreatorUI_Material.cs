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

    public SK_Material skAsset_Mat;
    [ReadOnly] public SKUI_FilepathField inputfield_imagePath;
    [ReadOnly] public SKUI_InputField inputfield_imageName;

    public override string FileExtension
    {
        get
        {
            return "skmat";
        }
    }
    public override void Start()
    {
        base.Start();
        workspace_Parent.name = $"{this.GetType().ToString()}";
    }

    public override void InitiateUI()
    {
        text_Console.text = "";

        inputfield_imagePath = Shopkeeper.UI.DrawFileLocationField(parentContent, "Add 3d model: ", placeholderText: "Select file location...");
        //inputfield_imagePath.button.onClick.AddListener(ClickOpen_FieldLocation);

        inputfield_imageName = Shopkeeper.UI.DrawTextInputField(parentContent, "File name: ", placeholderText: "Enter name...");
        //inputfield_imageName.inputfield.onEndEdit.AddListener(OnEndEdit_InputField);
    }

    public override void Button_SaveFile()
    {

    }

    public override void Button_LoadFile()
    {
        skAsset_Mat = LoadFile<SK_Material>(FileExtension);
        inputfield_imageName.inputfield.text = skAsset_Mat.fileName;

        RefreshUI();
    }

 
}
