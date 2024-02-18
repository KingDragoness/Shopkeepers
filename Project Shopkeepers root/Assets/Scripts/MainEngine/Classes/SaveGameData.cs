using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;


[System.Serializable]
public class BuildData
{

    //Level 0,1,2,3 (multi storey buildings)
    public int level = 0;

    [System.Serializable]
    public class WallDot
    {
        public Vector2Int pos;
        public List<Vector2Int> connectedDots = new List<Vector2Int>(); //only four possible connections

        public WallDot(Vector2Int pos)
        {
            this.pos = pos;
        }

        public void AddConnection(Vector2Int dot)
        {
            if (connectedDots.Exists(x => x == dot) == false)
            {
                connectedDots.Add(dot);
            }
        }
    }

    public List<WallDot> allWalls = new List<WallDot>();


}


[System.Serializable]
public class LotData
{
    public string lotName = "New Green 2910";
    public Vector2Int lotSize = new Vector2Int(40, 40);
    public List<BuildData> floorplanData = new List<BuildData>();
}

/// <summary>
/// Save game data folder
/// </summary>

[System.Serializable]
public class SaveGameData
{

    public string str_PlayerName = "Immortality";
    public string str_CompanyName = "Pukimart";
    public long long_Money = 20000;
    public int unixTime = 12592030;
    public string str_MainCategory = "Furniture";
    public List<LotData> allLotsOwned = new List<LotData>();

}
