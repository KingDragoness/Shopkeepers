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
    [ReadOnly] public BuildData.WallData wallData;

}
