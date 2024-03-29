﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using ToolBox.Pools;

public class BuildMode_Wall : BuildToolScript
{

    [FoldoutGroup("DEBUG")] public GameObject debug_wallPillar;
    [FoldoutGroup("DEBUG")] public LineRenderer debug_line;
    [FoldoutGroup("DEBUG")] public LineRenderer debug_linev2;
    [FoldoutGroup("DEBUG")] public bool DEBUG_Allow_GenerateDebugs = true;

    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_Simple;
    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_Simplev2;
    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_ExtraL;
    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_ExtraR;
    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_2DotCorner;
    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_Triangle;

    private Vector3Int startingWallPoint = new Vector3Int();
    [SerializeField] [ReadOnly] private BuildData previewFloorPlan = new BuildData();
    [SerializeField] [ReadOnly] private BuildData previewFloorPlan_beforeInvalid = new BuildData();
    [SerializeField] [ReadOnly] private int distance = 1;
    [SerializeField] [ReadOnly] private bool isInvalidDraw = false;
    [SerializeField] [ReadOnly] private bool isDeleteMode = false;
    private Vector2Int[] newDots = new Vector2Int[0];
    private BuildData.WallData newWallDat = new BuildData.WallData();

    private void Awake()
    {
        prefab_Wall_Simple.Populate(10);
        prefab_Wall_Simplev2.Populate(10);
        prefab_Wall_ExtraL.Populate(10);
        debug_line.gameObject.Populate(10);
        //debug_linev2.gameObject.Populate(10);


    }

    private void Start()
    {
        previewFloorPlan = Lot.MyLot.floorplanData[Shopkeeper.Game.currentLevel].Clone();
        previewFloorPlan_beforeInvalid = Lot.MyLot.floorplanData[Shopkeeper.Game.currentLevel].Clone();
    }

    private Vector3Int prevPositionArrow = new Vector3Int();

    public override void RunTool()
    {
        isDeleteMode = false;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isDeleteMode = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //set initial
            startingWallPoint = Shopkeeper.BuildMode.Arrow.GetArrowPosition();
            newDots = new Vector2Int[0];
            previewFloorPlan.allWallDatas.SetWallDatas(Lot.MyLot.floorplanData[Shopkeeper.Game.currentLevel].allWallDatas);
            previewFloorPlan_beforeInvalid.allWallDatas.SetWallDatas(Lot.MyLot.floorplanData[Shopkeeper.Game.currentLevel].allWallDatas);

        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (newDots.Length > 0)
            {
                DrawNewDots(newDots, isDeleteMode, BuildMode.CurrentFloorPlan.allWallDatas);
            }

            newDots = new Vector2Int[0];
        }
        else if(Input.GetMouseButton(0))
        {
            Vector3 currentArrowPos = Shopkeeper.BuildMode.Arrow.GetArrowPosition();
            distance = Mathf.RoundToInt((currentArrowPos - startingWallPoint).magnitude);
            newDots = new Vector2Int[distance + 1];
            if (distance < 1) newDots = new Vector2Int[0];
            Vector3 dir = (currentArrowPos - startingWallPoint).normalized;

            if (dir.OnlySingleDirection() && distance >= 1)
            {
                isInvalidDraw = false;
                for (int x = 0; x <= distance; x++)
                {
                    Vector3 ffs = startingWallPoint + (x * dir);
                    newDots[x].x = Mathf.RoundToInt(ffs.x);
                    newDots[x].y = Mathf.RoundToInt(ffs.z);

                }

            }
            else
            {
                isInvalidDraw = true;
            }

            if (isInvalidDraw)
            {
                previewFloorPlan.allWallDatas.SetWallDatas(previewFloorPlan_beforeInvalid.allWallDatas);
                newDots = new Vector2Int[0];
            }


            if (prevPositionArrow != Shopkeeper.BuildMode.Arrow.GetArrowPosition())
            {
                previewFloorPlan.allWallDatas.SetWallDatas(previewFloorPlan_beforeInvalid.allWallDatas);
                DrawNewDots(newDots, isDeleteMode);
            }


            //paint dots on the map
            Shopkeeper.BuildMode.Arrow.SetArrowColorUsed();
            prevPositionArrow = Shopkeeper.BuildMode.Arrow.GetArrowPosition();

        }
        else
        {
            Shopkeeper.BuildMode.Arrow.SetArrowColorIdle();

        }

    }

    public void DrawNewDots(Vector2Int[] allVector2, bool isDeleteMode = false, List<BuildData.WallData> toOverride = null)
    {
        List<Vector2Int> list_v2 = allVector2.ToList();
        List<BuildData.WallData> allModifiedWalls = new List<BuildData.WallData>();
        bool is_X = false;
        bool is_Y = false;

        //if deletemode, the list is walls that wants to be deleted.

        if (allVector2.Length >= 2)
        {
            if (Mathf.Abs(allVector2[1].x - allVector2[0].x) > 0.1f)
            {
                is_X = true;
            }
            if (Mathf.Abs(allVector2[1].y - allVector2[0].y) > 0.1f)
            {
                is_Y = true;
            }

            //get all lines that needs to be drawn
            //#1 (0,0) (1,0) (2,0) = we only need 2 wallData 0,0 and 0,1
            //#2 (4,0) (3,0) (2,0) = 2,0 and 3,0
            Vector2Int highestPos = list_v2.OrderBy(p => (p.x + p.y)).Last();

            foreach (var pos in allVector2)
            {
                if (pos.x == highestPos.x && pos.y == highestPos.y) continue;

                BuildData.WallData similarWall = previewFloorPlan.allWallDatas.Find(x => x.pos.x == pos.x && x.pos.y == pos.y); //always exists

                if (is_X) similarWall.x_wall = true;
                if (is_Y) similarWall.y_wall = true;
                if (is_X && isDeleteMode) { similarWall.x_wall = false; similarWall.wallpaperAssetPath_x_aSide = ""; similarWall.wallpaperAssetPath_x_bSide = ""; }
                if (is_Y && isDeleteMode) { similarWall.y_wall = false; similarWall.wallpaperAssetPath_y_aSide = ""; similarWall.wallpaperAssetPath_y_bSide = ""; }

                allModifiedWalls.Add(similarWall);
            }

        }
       

        if (toOverride != null)
        {
            toOverride.SetWallDatas(previewFloorPlan.allWallDatas);
            //toOverride.RemoveAllEmptyWalls();
            RenderWalls(toOverride, allModifiedWalls);
        }
        else
        {
            RenderWalls(previewFloorPlan.allWallDatas, allModifiedWalls);
        }

    }


    private void OnGUI()
    {
        if (Shopkeeper.Game.DEBUG_ALLOW_DEBUG_GUI == false) return;

        GUI.Label(new Rect(10, 20, 100, 20), $"isInvalidDraw:{isInvalidDraw}");
    }

    private List<GameObject> allWallTests = new List<GameObject>();
    private List<GameObject> allWallExtraTests = new List<GameObject>();
    private List<LineRenderer> allLineTests = new List<LineRenderer>();
    private List<Vector3> alreadyDrawnLines = new List<Vector3>();


    #region Generate raw wall
    public void ReRenderWallpaperOnly(List<BuildData.WallData> allWallDots)
    {
        foreach (var wall in allWallTests)
        {
            WallObject wallobj = wall.GetComponentInChildren<WallObject>();
            if (wallobj == null) continue;

            var new_WallDat = allWallDots.IsWallDataExistAt(wallobj.wallData.pos);
            if (new_WallDat == null) continue;
            wallobj.wallData.CopyData(new_WallDat);
            BuildMode.Wallpaper.PaintWall(wallobj);
        }

        foreach (var wall in allWallExtraTests)
        {
            WallObject wallobj = wall.GetComponentInChildren<WallObject>();

            var new_WallDat = allWallDots.IsWallDataExistAt(wallobj.wallData.pos);

            if (new_WallDat == null) continue;
            wallobj.wallData.CopyData(new_WallDat);
            BuildMode.Wallpaper.PaintExtraWall(wallobj, allWallDots);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="allWallDots"></param>
    /// <param name="optimizedRendering">Only change any unchanged walls.</param>
    public void RenderWalls(List<BuildData.WallData> allWallDots, List<BuildData.WallData> allModifiedWalls)
    {

        allWallTests.ReleasePoolObject();
        allWallExtraTests.ReleasePoolObject();
        allLineTests.ReleasePoolObject();
        alreadyDrawnLines.Clear();
        List<BuildData.WallData> z_listDots = new List<BuildData.WallData>();
        z_listDots.AddRange(allWallDots);
        z_listDots.RemoveAll(x => !x.x_wall && !x.y_wall);

        if (DEBUG_Allow_GenerateDebugs)
        {

            foreach (var wallDot in z_listDots)
            {
                Vector3 pos = new Vector3(wallDot.pos.x, 0f, wallDot.pos.y);

                var newWallprefab = debug_wallPillar.Reuse(transform);//Instantiate(debug_wallPillar, transform);
                newWallprefab.gameObject.SetActive(true);
                newWallprefab.transform.position = pos;
                allWallTests.Add(newWallprefab);

                if (wallDot.x_wall)
                {
                    var newWireLine = debug_line.gameObject.Reuse(transform).GetComponent<LineRenderer>();//Instantiate(debug_line, transform);
                    Vector3[] positions = new Vector3[2];
                    positions[0] = new Vector3(wallDot.pos.x, 0.02f, wallDot.pos.y);
                    positions[1] = new Vector3(wallDot.NextX().x, 0.02f, wallDot.pos.y);

                    newWireLine.gameObject.SetActive(true);
                    newWireLine.SetPositions(positions);
                    allLineTests.Add(newWireLine);
                    alreadyDrawnLines.Add(positions[0]);
                    alreadyDrawnLines.Add(positions[1]);
                }

                if (wallDot.y_wall)
                {
                    var newWireLine = debug_line.gameObject.Reuse(transform).GetComponent<LineRenderer>();//Instantiate(debug_line, transform);
                    Vector3[] positions = new Vector3[2];
                    positions[0] = new Vector3(wallDot.pos.x, 0.02f, wallDot.pos.y);
                    positions[1] = new Vector3(wallDot.pos.x, 0.02f, wallDot.NextY().y);

                    newWireLine.gameObject.SetActive(true);
                    newWireLine.SetPositions(positions);
                    allLineTests.Add(newWireLine);
                    alreadyDrawnLines.Add(positions[0]);
                    alreadyDrawnLines.Add(positions[1]);
                }
            }

        }

        foreach (var wallDot in z_listDots)
        {
            if (!wallDot.x_wall && !wallDot.y_wall) continue;

            //if (allModifiedWalls.Find(x => x.pos.x == wallDot.pos.x) != null)
            //{
            //    var similarData = previewFloorPlan.allWallDatas.Find(x => x.pos.x == wallDot.pos.x && x.pos.y == wallDot.pos.y);
            //    if (wallDot.IsSimilarWith(similarData))
            //    {
            //        //continue;
            //    }
            //}

            for (int z1 = 0; z1 < 2; z1++)
            {
                if (z1 == 0 && wallDot.x_wall == false) continue;
                if (z1 == 1 && wallDot.y_wall == false) continue;

                Vector3 pos = new Vector3(wallDot.pos.x, 0f, wallDot.pos.y);
                GameObject selectedPrefab = prefab_Wall_Simple;

                if (pos.x % 2 == 0 | pos.y % 2 == 0)
                {
                    selectedPrefab = prefab_Wall_Simplev2;
                }

                var wall = selectedPrefab.Reuse(Shopkeeper.Scene); //Instantiate(selectedPrefab, Shopkeeper.Scene);
                WallObject wallObj = wall.GetComponentInChildren<WallObject>();
                Vector3 _position = new Vector3(wallDot.pos.x, 0, wallDot.pos.y);
                Vector3 _rotation = Vector3.zero;
                wall.gameObject.SetActive(true);
                {
                    if (wallObj.wallData == null) wallObj.wallData = new BuildData.WallData();
                    wallObj.wallData.CopyData(wallDot, true); //BAD COPYING DATA

                    if (wallObj.wallData.pos != wallDot.pos)
                    {
                        Debug.Log($"bad copying: {wallObj.wallData.pos} {wallDot.pos}");
                    }
                }

                if (z1 == 0 && wallDot.x_wall)
                {
                    _rotation = new Vector3(0, 0, 0);
                    wallObj.currentWallFacing = WallObject.WallFacing.X_Axis;
                }
                if (z1 == 1 && wallDot.y_wall)
                {
                    _rotation = new Vector3(0, -90, 0);
                    wallObj.currentWallFacing = WallObject.WallFacing.Y_Axis;
                }

                BuildMode.Wallpaper.PaintWall(wallObj);

                wall.transform.position = _position;
                wall.transform.eulerAngles = _rotation;

                allWallTests.Add(wall);
            }

        }

        GenerateExtraWalls(allWallDots);
    }

    /// <summary>
    /// Fill in any holes
    /// </summary>
    public void GenerateExtraWalls(List<BuildData.WallData> allWallDots)
    {
        //var extraWall = Instantiate(prefab, transform);
        //extraWall.gameObject.SetActive(true);
        //Vector3 _position = new Vector3(wallDot.pos.x, 0, wallDot.pos.y);

        //extraWall.transform.position = _position;
        //extraWall.transform.eulerAngles = _rotation;
        //allWallExtraTests.Add(extraWall);

        foreach (var wallDot in allWallDots)
        {
            if (wallDot.x_wall == false && wallDot.y_wall == false) continue;

            //check all side connections
            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;
            int connections = 0;
            var prevX_Wall = allWallDots.IsWallDataExistAt(wallDot.PrevX());
            var prevY_Wall = allWallDots.IsWallDataExistAt(wallDot.PrevY());

            if (wallDot.x_wall == true) { right = true; connections++; }
            if (wallDot.y_wall == true) { up = true; connections++; }
            if (prevX_Wall != null) { if (prevX_Wall.x_wall) { left = true; connections++; } }
            if (prevY_Wall != null) { if (prevY_Wall.y_wall) { down = true; connections++; } }

            if (connections <= 9)
            {
                var extraWall = prefab_Wall_ExtraL.Reuse(Shopkeeper.Scene); //Instantiate(prefab_Wall_ExtraL, Shopkeeper.Scene);
                extraWall.gameObject.SetActive(true);
                WallObject wallObj = extraWall.GetComponentInChildren<WallObject>();
                {
                    wallObj.ResetYouFcukingRetard();
                    if (wallObj.wallData == null) wallObj.wallData = new BuildData.WallData();
                    wallObj.wallData.CopyData(wallDot, true);
                }
                BuildMode.Wallpaper.PaintExtraWall(wallObj, allWallDots);

                Vector3 _position = new Vector3(wallDot.pos.x, 0, wallDot.pos.y);

                extraWall.transform.position = _position;
                allWallExtraTests.Add(extraWall);
            }
        }


        foreach (var wallD in allWallDots)
        {
            BuildData.WallData cornerUpWall = allWallDots.Find(x => x.pos == wallD.NextY());

            //we are pivot from the cornerupwall
            if (cornerUpWall != null)
            {
                bool left = false;
                bool right = false;
                bool up = false;
                bool down = false;
                int connections = 0;
                var prevX_Wall = allWallDots.IsWallDataExistAt(cornerUpWall.PrevX());
                var prevY_Wall = allWallDots.IsWallDataExistAt(cornerUpWall.PrevY());

                if (cornerUpWall.x_wall == true) { continue; } //any wall in the right and up are banned
                if (cornerUpWall.y_wall == true) { continue; }
                if (prevX_Wall != null) { if (prevX_Wall.x_wall) { left = true; connections++; } }
                if (prevY_Wall != null) { if (prevY_Wall.y_wall) { down = true; connections++; } }

                if (connections == 2) //precisely only two connections, we generate it.
                {
                    var prevX_wall = allWallDots.IsWallDataExistAt(cornerUpWall.PrevX());
                    var extraWall = prefab_Wall_ExtraL.Reuse(Shopkeeper.Scene); //Instantiate(prefab_Wall_ExtraL, Shopkeeper.Scene);
                    extraWall.gameObject.SetActive(true);
                    WallObject wallObj = extraWall.GetComponentInChildren<WallObject>();
                    wallObj.ResetYouFcukingRetard();
                    wallObj.wallData.CopyData(wallD, true); //We clone from the original pivot
                    wallObj.wallData.wallpaperAssetPath_x_aSide = prevX_wall.wallpaperAssetPath_x_aSide;
                    wallObj.wallData.wallpaperAssetPath_x_bSide = prevX_wall.wallpaperAssetPath_x_bSide;
                    wallObj.cornerXY_wall = true;
                    BuildMode.Wallpaper.PaintExtraWall(wallObj, allWallDots);

                    Vector3 _position = new Vector3(cornerUpWall.pos.x, 0, cornerUpWall.pos.y);

                    extraWall.transform.position = _position;
                    allWallExtraTests.Add(extraWall);
                }
            }
        }
    }


    #endregion

    
    public enum WallCornerType
    {
        none,
        _x_y,
        xy,
        _xy,
        x_y
    }

    public WallCornerType IsExtraWall_a_Corner(WallObject wallObject, List<BuildData.WallData> allWallDots = null)
    {
        if (allWallDots == null) return WallCornerType.none;

        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;
        int connections = 0;
        var prevX_Wall = allWallDots.IsWallDataExistAt(wallObject.wallData.PrevX());
        var prevY_Wall = allWallDots.IsWallDataExistAt(wallObject.wallData.PrevY());


        if (wallObject.wallData.x_wall == true) 
        { 
            right = true;  
            connections++;
        } 

        if (wallObject.wallData.y_wall == true) 
        { 
            up = true;  
            connections++; 
        }

        wallObject.reportTest = $"{wallObject.wallData.pos}";

        if (prevX_Wall != null) 
        {
            wallObject.reportTest += $"{prevX_Wall.pos} | x wall: {prevX_Wall.x_wall}";
            if (prevX_Wall.x_wall == true) 
            { 
                left = true; 
                connections++;
                //Debug.Log($"{left} =  {prevX_Wall.pos} | x wall: {prevX_Wall.x_wall}");
            }

            //Debug.Log($"test1 {wallObject.wallData.pos}");
        }
        if (prevY_Wall != null) 
        { 
            if (prevY_Wall.y_wall == true) 
            { 
                down = true; 
                connections++; 
            }
            wallObject.reportTest = $"{wallObject.wallData.pos} | Prev y wall: {prevY_Wall.pos}";
        }

        wallObject.connections = connections;
        wallObject.left = left;
        wallObject.right = right;
        wallObject.up = up;
        wallObject.down = down;

        if (connections == 2)
        {
            if (left && down) return WallCornerType._x_y;
            if (right && down) return WallCornerType.x_y;
            if (left && up) return WallCornerType._xy;
            if (right && up) return WallCornerType.xy;

        }

     
        return WallCornerType.none;
    }

    public int GetTotalConnections(WallObject wallObject, List<BuildData.WallData> allWallDots = null)
    {
        if (allWallDots == null) return 0;

        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;
        int connections = 0;
        var prevX_Wall = allWallDots.IsWallDataExistAt(wallObject.wallData.PrevX());
        var prevY_Wall = allWallDots.IsWallDataExistAt(wallObject.wallData.PrevY());

        if (wallObject.wallData.x_wall == true) { right = true; connections++; }
        if (wallObject.wallData.y_wall == true) { up = true; connections++; }
        if (prevX_Wall != null) { if (prevX_Wall.x_wall) { left = true; connections++; } }
        if (prevY_Wall != null) { if (prevY_Wall.y_wall) { down = true; connections++; } }
 

        return connections;
    }

}
