using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class ShopkeeperGame : MonoBehaviour
{

    public enum SpeedType
    {
        Paused, 
        Normal, //x1
        Faster, //x2
        Fastest //x4
    }

    public long money = 1552820;
    public int unixTime = 1708063371;
    public float timescaleIngame = 20f;
    public GamemodeSO Gamemode;
    public SaveGameData savedata;
    public SpeedType speedType;

    private float deltaUnixTAdded = 0f;


    [FoldoutGroup("DEBUG")] public Item DEBUGTest_Item;
    [FoldoutGroup("DEBUG")] [TextArea(3, 6)] public string DEBUGTest_JsonData = "";

    [FoldoutGroup("DEBUG")] [Button("DEBUG Asset Pack")]
    public void DEBUG_AssetPack()
    {
        DEBUGTest_JsonData = Shopkeeper.AssetCreator.ConvertSKAssetToJSON<Item>(DEBUGTest_Item);
    }

    private void Start()
    {
        NewGameStart();
    }

    private void NewGameStart()
    {
        timescaleIngame = Gamemode.timescaleSpeed;
        DateTime dateTime = new DateTime(Gamemode.start_Year, Gamemode.start_Month, Gamemode.start_Day, 8, 0, 0, 0, DateTimeKind.Local);
        money = Gamemode.startingMoney;
        unixTime = (int)dateTime.ConvertToUnixTimestamp();

    }

    private void Update()
    {
        HandleGameSpeed();
    }

    private void HandleGameSpeed()
    {
        int tries = 0;
        int speed = 1;

        if (speedType == SpeedType.Paused)
        {
            speed = 0;
        }
        else if (speedType == SpeedType.Normal)
        {
            speed = 1;
        }
        else if (speedType == SpeedType.Faster)
        {
            speed = 2;
        }
        else if (speedType == SpeedType.Fastest)
        {
            speed = 4;
        }

        deltaUnixTAdded += Time.deltaTime * timescaleIngame * speed;

        while (deltaUnixTAdded > 1 && tries <= 10)
        {
            unixTime += Mathf.FloorToInt(deltaUnixTAdded);
            deltaUnixTAdded = 0;
            tries++;
        }
    }
}
