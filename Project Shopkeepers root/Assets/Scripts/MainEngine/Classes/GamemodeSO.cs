using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


/// <summary>
/// Only for use internally.
/// </summary>
/// 
[CreateAssetMenu(fileName = "Career", menuName = "Shopkeeper/Gamemode", order = 1)]

public class GamemodeSO : ScriptableObject
{

    public string gamemodeName = "Career";
    public string description = "";
    public int start_Year = 2020;
    public int start_Month = 1;
    public int start_Day = 10;
    public float minimumWorkStart = 6.5f; //Minimum shop start: 06:30
    public float maximumWorkStart = 19.5f; //Maximum shop end: 20:00 (13h30m total)
    public long startingMoney = 55000;
    public float timescaleSpeed = 30f;

}
