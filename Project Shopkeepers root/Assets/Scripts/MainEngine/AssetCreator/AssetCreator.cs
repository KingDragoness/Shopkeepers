using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using Newtonsoft.Json;

public class AssetCreator : MonoBehaviour
{

    #region Developer Mode
    [InfoBox("Always at /StreamingAssets/Category")]
    public Category category;

    [FoldoutGroup("DEVELOPER")] [Button("DevMode!_BuildCategoryAsset")]

    public void DEVMODE_BuildCategoryAsset()
    {
        string path = $"{Shopkeeper.Path_StreamingAssets}/Category/{category.b_DisplayName}";
        string content = Shopkeeper.AssetCreator.ConvertSKAssetToJSON<Category>(category);
        Shopkeeper.Database.SaveAssetFile(path, category.Extension, content);
    }

    #endregion

    public string ConvertSKAssetToJSON<T>(object value) where T : SKAsset
    {
        string contentData = JsonConvert.SerializeObject(value as T, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        });

        return contentData;
    }

}
