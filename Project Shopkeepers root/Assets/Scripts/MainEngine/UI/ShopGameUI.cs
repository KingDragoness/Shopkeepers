using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;


public enum GameUIType
{
    None,
    Perks,
    Management,
    Inventory,
    Finance,
    Realestate,
    BuildMode
}


public class ShopGameUI : MonoBehaviour
{

    public Text label_Date;
    public Text label_Time;
    public Text label_Money;

    [FoldoutGroup("Primary Tab")] public GameUIType currentUI;
    [FoldoutGroup("Primary Tab")] public PrimaryUITab UI_Perks;
    [FoldoutGroup("Primary Tab")] public PrimaryUITab UI_Management;
    [FoldoutGroup("Primary Tab")] public PrimaryUITab UI_Inventory;
    [FoldoutGroup("Primary Tab")] public PrimaryUITab UI_Finance;
    [FoldoutGroup("Primary Tab")] public PrimaryUITab UI_Realestate;
    [FoldoutGroup("Primary Tab")] public PrimaryUITab UI_Buildmode;



    private void Awake()
    {
        UI_Perks.label_header.text = "Perks";
        UI_Perks.image_icon.sprite = Shopkeeper.InternalAsset.Icon_Perk;
        UI_Management.label_header.text = "Management";
        UI_Management.image_icon.sprite = Shopkeeper.InternalAsset.Icon_Management;
        UI_Inventory.label_header.text = "Inventory";
        UI_Inventory.image_icon.sprite = Shopkeeper.InternalAsset.Icon_Inventory;
        UI_Finance.label_header.text = "Finance";
        UI_Finance.image_icon.sprite = Shopkeeper.InternalAsset.Icon_Finance;
        UI_Realestate.label_header.text = "Real Estate";
        UI_Realestate.image_icon.sprite = Shopkeeper.InternalAsset.Icon_Realestate;
        UI_Buildmode.label_header.text = "Build/Furnish Mode";
        UI_Buildmode.image_icon.sprite = Shopkeeper.InternalAsset.Icon_Buildmode;
    }

    public void OpenUI(int type)
    {
        if (currentUI == (GameUIType)type)
        {
            type = 0;
        }

        currentUI = (GameUIType)type;
    }

    private void Update()
    {
        UpdatePrimaryTabs();
        UpdateLabels();
    }

    private void UpdateLabels()
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(Shopkeeper.Game.unixTime).ToLocalTime();

        label_Money.text = $"${Shopkeeper.Game.money:n0}";
        label_Time.text = $"{dateTime.ToString("HH:mm tt")}";
        label_Date.text = $"{dateTime.ToString("dd MMMM yyyy")}";
    }

    private void UpdatePrimaryTabs()
    {
        if (currentUI == GameUIType.Perks)
        {
            UI_Perks.gameObject.EnableGameobject(true);
        }
        else
        {
            UI_Perks.gameObject.EnableGameobject(false);
        }

        if (currentUI == GameUIType.Management)
        {
            UI_Management.gameObject.EnableGameobject(true);
        }
        else
        {
            UI_Management.gameObject.EnableGameobject(false);
        }

        if (currentUI == GameUIType.Inventory)
        {
            UI_Inventory.gameObject.EnableGameobject(true);
        }
        else
        {
            UI_Inventory.gameObject.EnableGameobject(false);
        }

        if (currentUI == GameUIType.Finance)
        {
            UI_Finance.gameObject.EnableGameobject(true);
        }
        else
        {
            UI_Finance.gameObject.EnableGameobject(false);
        }

        if (currentUI == GameUIType.Realestate)
        {
            UI_Realestate.gameObject.EnableGameobject(true);
        }
        else
        {
            UI_Realestate.gameObject.EnableGameobject(false);
        }

        if (currentUI == GameUIType.BuildMode)
        {
            UI_Buildmode.gameObject.EnableGameobject(true);
        }
        else
        {
            UI_Buildmode.gameObject.EnableGameobject(false);
        }
    }
}
