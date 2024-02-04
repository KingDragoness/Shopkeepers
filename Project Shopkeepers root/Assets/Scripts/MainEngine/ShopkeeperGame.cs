using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ShopkeeperGame : MonoBehaviour
{

    public Item DEBUGTest_Item;
    [TextArea(3, 6)] public string DEBUGTest_JsonData = "";

    [Button("DEBUG Asset Pack")]
    public void DEBUG_AssetPack()
    {
        DEBUGTest_JsonData = Shopkeeper.AssetCreator.ConvertSKAssetToJSON<Item>(DEBUGTest_Item);
    }

}
