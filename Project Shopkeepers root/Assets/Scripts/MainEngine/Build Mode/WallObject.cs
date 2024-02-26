using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WallObject : MonoBehaviour
{

    public enum WallFacing
    {
        X_Axis,
        Y_Axis
    }

    public enum Side
    {
        A_Side,
        B_Side
    }

    public WallFacing currentWallFacing;
    public MeshRenderer meshRndrWall;
    public bool cornerXY_wall = false;
    public BuildMode_Wall.WallCornerType cornerType;
    public int connections = 0;
    [FoldoutGroup("dir")] public bool left = false;
    [FoldoutGroup("dir")] public bool right = false;
    [FoldoutGroup("dir")] public bool up = false;
    [FoldoutGroup("dir")] public bool down = false;
    [FoldoutGroup("dir")] public string reportTest = "";

    //I FUCKING HATE POOLING
    public void ResetYouFcukingRetard()
    {
        connections = 0;
        cornerXY_wall = false;
        left = false;
        right = false;
        up = false;
        down = false;
        cornerType = BuildMode_Wall.WallCornerType.none;
    }

    [ReadOnly] public BuildData.WallData wallData;

}
