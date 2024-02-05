using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
/// </summary>


public class DatabaseAssetHandler : MonoBehaviour
{

    public List<string> all_LoadPathTarget = new List<string>();

    #region Loading Asset
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
    /// <param name="extension">Extension file (.sk3d, .skmat)</param>
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
