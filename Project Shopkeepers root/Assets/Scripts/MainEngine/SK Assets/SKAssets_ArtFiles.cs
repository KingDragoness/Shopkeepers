using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;


[Serializable]
public class SK_3dModel : SKAsset
{

    public List<string> filePath_3dModelPaths = new List<string>(); //.obj file (/IKEA Furniture Packs/Chairs/Poang/PoangArmchair.obj)
    public List<string> filePath_MaterialPaths = new List<string>(); //.skmat file (/IKEA Furniture Packs/Chairs/Poang/Mat_PoangArmchair.skmat)
    [ReadOnly] public GameObject loadedAsset; //run-time only

    public override string Extension
    {
        get
        {
            return ".sk3d";
        }
    }

    [Button("Load Test")]
    public void Load3dObj()
    {
        Shopkeeper.Database.Load_SK3dmodel(this);
    }
}


public class OnlyATest : MonoBehaviour
{
    public void Test1()
    {
        Material z = GetComponent<MeshRenderer>().material;

    }
}

[Serializable]
public class SK_Material: SKAsset
{

    //for now it's simple
    public string filePath_MainTexture = ""; ///IKEA Furniture Packs/Chairs/Poang/Texture_PoangArmchair.png
    public string filePath_Normalmap = ""; ///IKEA Furniture Packs/Chairs/Poang/Texture_PoangArmchair_Normalmap.png
    public Vector3Int AlbedoColor = new Vector3Int(255,255,255); //white 0-255, 8-bit
    [ReadOnly] public Material loadedAsset; //run-time only


    public override string Extension
    {
        get
        {
            return ".skmat";
        }
    }
}


[Serializable]
public class SK_Texture : SKAsset
{

    public string filePath_MainTexture = "/IKEA Furniture Packs/Chairs/Poang/Thumbnail_PoangArmchair.png";
    public bool isWallpaper = false;
    public bool isFlooring = false;
    [ReadOnly] public Texture2D loadedAsset; //run-time only
    [ReadOnly] public Material generatedMaterial; //run-time only


    public override string Extension
    {
        get
        {
            return ".sktex";
        }
    }

    [Button("Load texture")]
    public void LoadTexture()
    {
        Shopkeeper.Database.Load_SKTexture(this);
    }
}

//build assets
[Serializable]
public class SK_Wallpaper : SKAsset
{

    public string filePath_Texture = ""; ///IKEA Furniture Packs/Wallpapers/STex_Wallpaper_Royal.sktex


    public override string Extension
    {
        get
        {
            return ".skwallpaper";
        }
    }
}