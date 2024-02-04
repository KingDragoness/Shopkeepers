using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;


[Serializable]
public class Category : SKAsset
{


    public string b_DisplayName = "Furniture";
    public string b_Description = "Furniture is household equipment, usually made of wood, metal, plastics, marble, glass, fabrics, or related materials and having a variety of different purposes.";

    public override string Extension
    {
        get
        {
            return ".skcat";
        }
    }
}
