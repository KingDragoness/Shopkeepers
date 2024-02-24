using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Dummiesman; //Load OBJ Model
using SFB; //Load OBJ Model

public class AssetCreatorUI_Wallpaper : AssetCreatorWindow
{

    public SK_Wallpaper skAsset_wallpaper;

    [ReadOnly] public SKUI_FilepathField inputfield_texPath;

    public void InitiateUI()
    {
        text_Console.text = "";

        inputfield_texPath = Shopkeeper.UI.DrawFileLocationField(parentContent, "Add SK texture: ", placeholderText: "Select file location...");
        inputfield_texPath.button.onClick.AddListener(ClickOpen_FieldLocation);

    }

    public void ClickOpen_FieldLocation()
    {
        var extensions = new[] {
                new ExtensionFilter("SK Texture", "sktex" ),
            };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions: extensions, false);
        if (paths.Length > 0)
        {
            var uri = new System.Uri(paths[0]);
            skAsset_wallpaper.filePath_Texture = Shopkeeper.Database.ProcessUriPath(uri.LocalPath);
            RefreshUI();
            //RefreshMat();
        }
    }




    public override void Button_LoadFile()
    {
        throw new System.NotImplementedException();
    }

    public override void Button_SaveFile()
    {
        throw new System.NotImplementedException();
    }
}
