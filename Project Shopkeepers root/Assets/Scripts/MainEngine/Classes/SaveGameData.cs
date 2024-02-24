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
    public class WallData
    {
        public Vector2Int pos;
        public bool x_wall = false;
        public bool y_wall = false;

        public bool isBothWalls()
        {
            if (x_wall && y_wall) return true;

            return false;
        }

        //x_aSide wallpaper
        //x_bSide wallpaper
        public string wallpaperAssetPath_x_aSide = "";
        public string wallpaperAssetPath_x_bSide = "";
        public string wallpaperAssetPath_y_aSide = "";
        public string wallpaperAssetPath_y_bSide = "";

        public WallData(Vector2Int pos, bool _hasXwall, bool _hasYwall)
        {
            this.pos = pos;
            this.x_wall = _hasXwall;
            this.y_wall = _hasYwall;
        }

        public Vector2Int PrevOffset(Vector2Int offset)
        {
            return pos + offset;
        }
        public Vector2Int PrevX()
        {
            return pos + new Vector2Int(-1, 0);
        }
        public Vector2Int PrevY()
        {
            return pos + new Vector2Int(0, -1);
        }
        public Vector2Int NextX()
        {
            return pos + new Vector2Int(1,0);
        }
        public Vector2Int NextY()
        {
            return pos + new Vector2Int(0,1);
        }

        public Vector2Int CornerUpXY()
        {
            return pos + new Vector2Int(1, 1);
        }

    }

    public List<WallData> allWallDatas = new List<WallData>();


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
