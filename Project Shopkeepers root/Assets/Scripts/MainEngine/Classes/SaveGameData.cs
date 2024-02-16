using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;


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


}
