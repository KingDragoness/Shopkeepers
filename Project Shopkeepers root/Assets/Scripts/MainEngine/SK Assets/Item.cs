using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// SKAsset Item files:
/// 3d models: "/IKEA Furniture Packs/Chairs/Poang/PoangArmchair.obj" (optional, loaded from prefab)
/// 3d model's material: "/IKEA Furniture Packs/Chairs/Poang/Mat_PoangArmchair.skmat" (optional, loaded from prefab)
/// prefabs: "/IKEA Furniture Packs/Chairs/Poang/IKEA Poang Armchair.sk3d" (optional)
/// diffuse texture: "/IKEA Furniture Packs/Chairs/Poang/Texture_PoangArmchair.png" (optional, from material)
/// normalmap texture: "/IKEA Furniture Packs/Chairs/Poang/Texture_PoangArmchair_Normalmap.png" (optional, from material)
/// packaging texture: "/IKEA Furniture Packs/Chairs/Poang/Texture_Packaging_PoangArmchair.sktex" (Required)
/// thumbnail/screenshot texture: "/IKEA Furniture Packs/Chairs/Poang/Thumbnail_PoangArmchair.png" (Required)
/// </summary>


[Serializable]
public class Item : SKAsset
{

    public Vector3 itemDimension = new Vector3(1f,1f,1f);
    public float factoryPrice = 100f; //The factory sells this item for $100.
    public string b_Name = "IKEA POÄNG Armchair";
    public string b_Description = "POÄNG armchair has stylish curved lines in bentwood, providing nice support for the neck and comfy resilience.";
    public string[] b_ItemTags;
    public string d_Category = "Furniture";
    public string filePath_Prefab = "/IKEA Furniture Packs/Chairs/Poang/IKEA Poang Armchair.sk3d"; //optional
    public string filePath_PackagingTexture = "/IKEA Furniture Packs/Chairs/Poang/Texture_Packaging_PoangArmchair.sktex"; //REQUIRED!
    public string[] filePath_Screenshots;
    public int b_ShelfLife = 65535; //Bread 5-10 days shelf life.
    public bool b_RequireRefrigeration = false;

    public override string Extension
    {
        get
        {
            return ".skitem";
        }
    }

}
