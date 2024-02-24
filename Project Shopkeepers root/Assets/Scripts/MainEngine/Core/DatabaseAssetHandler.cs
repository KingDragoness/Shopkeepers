using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEngine;
using Dummiesman;
using SFB;
using UnityEngine.Networking;

/// <summary>
/// ASSET EXTENSION TYPES:
/// ".sk3d" = Shopkeeper 3d model file which is essentially a prefab.
/// ".skmat" = Shopkeeper material file which contains material parameters.
/// ".skitem" = Shopkeeper's item file
/// ".skcat" = Shopkeeper's category file
/// ".sktex" = Shopkeeper's texture file
/// ".skwallpaper" = Shopkeeper's wallpaper file
/// </summary>



public class DatabaseAssetHandler : MonoBehaviour
{

    public List<string> all_LoadPathTarget = new List<string>();
    public bool DEBUG_loadAllAssetMeta = false;
    public Shader defaultShader;
    [Space]
    //loaded database
    [Header("Loaded Assets")]
    public List<SK_3dModel> loaded_3dModelAsset = new List<SK_3dModel>();
    public List<SK_Material> loaded_MaterialsAsset = new List<SK_Material>();
    public List<SK_Texture> loaded_TextureAsset = new List<SK_Texture>();


    private void Awake()
    {
        if (DEBUG_loadAllAssetMeta == true)
        {
            LoadAllAssets();
        }
    }

    public List<DirectoryInfo> GetAllAssetPaths()
    {
        List<DirectoryInfo> allDirectoryInfos = new List<DirectoryInfo>();
        DirectoryInfo d = new DirectoryInfo($"{Shopkeeper.Path_StreamingAssets}");
        allDirectoryInfos.Add(d);


        return allDirectoryInfos;
    }

    [FoldoutGroup("DEBUG")]
    [Button("DEBUG_LoadAllAssets")]
    public void LoadAllAssets()
    {
        loaded_3dModelAsset.Clear();
        loaded_MaterialsAsset.Clear();
        loaded_TextureAsset.Clear();
        Resources.UnloadUnusedAssets();
        List<DirectoryInfo> allDirectoryInfos = GetAllAssetPaths();

        foreach (var dir in allDirectoryInfos)
        {
            //find sk3d
            {
                FileInfo[] Files = dir.GetFiles("*.sk3d", SearchOption.AllDirectories);

                foreach (FileInfo file in Files)
                {
                    try 
                    {
                        var skAsset = LoadAssetFile<SK_3dModel>(file.FullName, "");
                        loaded_3dModelAsset.Add(skAsset);

                    }
                    catch
                    {
                        Debug.LogError($"ERROR! Failed loading asset: {file.Name}");

                    }
                }
            }

            //find skmat
            {
                FileInfo[] Files = dir.GetFiles("*.skmat", SearchOption.AllDirectories);

                foreach (FileInfo file in Files)
                {
                    try
                    {
                        var skAsset = LoadAssetFile<SK_Material>(file.FullName, "");
                        loaded_MaterialsAsset.Add(skAsset);

                    }
                    catch
                    {
                        Debug.LogError($"ERROR! Failed loading asset: {file.Name}");

                    }
                }
            }

            {
                FileInfo[] Files = dir.GetFiles("*.sktex", SearchOption.AllDirectories);

                foreach (FileInfo file in Files)
                {
                    try
                    {
                        var skAsset = LoadAssetFile<SK_Texture>(file.FullName, "");
                        loaded_TextureAsset.Add(skAsset);

                    }
                    catch
                    {
                        Debug.LogError($"ERROR! Failed loading asset: {file.Name}");

                    }
                }
            }
        }
    }

    #region Loading Asset
    public GameObject Load_SK3dmodel(SK_3dModel sk_model)
    {
        if (sk_model.loadedAsset != null) return sk_model.loadedAsset;

        var allDirectoryInfos = GetAllAssetPaths();

        foreach (var directory in allDirectoryInfos)
        {
            foreach (var path in sk_model.filePath_3dModelPaths)
            {
                string pathURL = directory + path;
                sk_model.loadedAsset = Load3dModel(pathURL);
                return sk_model.loadedAsset;
            }
        }

        return null;
    }

    public Texture2D Load_SKTexture(SK_Texture sk_texture)
    {
        if (sk_texture.loadedAsset != null) return sk_texture.loadedAsset;

        var allDirectoryInfos = GetAllAssetPaths();

        foreach (var directory in allDirectoryInfos)
        {
            string pathURL = directory + sk_texture.filePath_MainTexture;
            sk_texture.loadedAsset = LoadTexture(pathURL);
            return sk_texture.loadedAsset;
        }

        return null;
    }

    public Material Load_Material(SK_Texture sk_texture)
    {
        if (sk_texture.generatedMaterial != null) return sk_texture.generatedMaterial;

        Texture2D texture2d = Load_SKTexture(sk_texture);
        sk_texture.generatedMaterial = new Material(defaultShader);
        sk_texture.generatedMaterial.SetTexture("_MainTex", texture2d);

        return sk_texture.generatedMaterial;
    }

    public GameObject Load3dModel(string url)
    {
        GameObject result3dModel = null;

        Debug.Log($"Loaded .obj file: {url}");


        if (!File.Exists(url))
        {
            Debug.LogError($"3d model is missing! ({url})");
        }
        else
        {

            result3dModel = new OBJLoader().Load(url);
            result3dModel.transform.localScale = new Vector3(1, 1, 1); // set the position of parent model. Reverse X to show properly 
        }

        return result3dModel;
    }

    public Texture2D LoadTexture(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }

        return tex;
    }
    #endregion


    #region SK Asset

    public string ProcessUriPath(string fullPath)
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

        return localPath;

    }

    /// <summary>
    /// Load the SK asset files.
    /// </summary>
    /// <param name="path">Full path including C: drives.</param>
    /// <param name="extension">Better not to fill this. Extension file (.sk3d, .skmat)</param>
    /// <returns></returns>
    public T LoadAssetFile<T>(string path = "", string extension = "") where T : SKAsset
    {
        string pathLoad = "";
        JsonSerializerSettings settings = JsonSettings();
        T assetFile = null;

        if (!string.IsNullOrWhiteSpace(extension))
        {
            pathLoad = $"{path}{extension}";
        }
        else
        {
            pathLoad = $"{path}";
        }

        try
        {
            assetFile = JsonConvert.DeserializeObject<T>(File.ReadAllText(pathLoad), settings);
        }
        catch
        {
            Debug.LogError($"Asset failed to load! ({path})");
        }


        return assetFile;

    }

    public SK_Texture Get_SKTexture(string path)
    {
        return loaded_TextureAsset.Find(x => x.filePath_MainTexture == path);
    }


    public void SaveAssetFile(string path = "", string extension = ".skitem", string content = "")
    {
        string pathSave = $"{path}{extension}";

        File.WriteAllText(pathSave, content);

    }

    public static JsonSerializerSettings JsonSettings()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        return settings;

    }
    #endregion

}
