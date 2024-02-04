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


    public override string Extension
    {
        get
        {
            return ".sk3d";
        }
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
    public string filePath_MainTexture = "/IKEA Furniture Packs/Chairs/Poang/Texture_PoangArmchair.png";
    public string filePath_Normalmap = "/IKEA Furniture Packs/Chairs/Poang/Texture_PoangArmchair_Normalmap.png";



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


    public override string Extension
    {
        get
        {
            return ".sktex";
        }
    }
}