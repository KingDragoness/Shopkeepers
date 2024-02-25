using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

[System.Serializable]
public class WallSelection
{
    public Vector2Int pos;
    public WallObject.Side wall_side = WallObject.Side.A_Side;
    public WallObject.WallFacing wall_axis = WallObject.WallFacing.X_Axis;

    public enum Type
    {
        none,
        xa,
        xb,
        ya,
        yb
    }

    public WallSelection(Vector2Int pos, WallObject.Side wall_side, WallObject.WallFacing wall_axis)
    {
        this.pos = pos;
        this.wall_side = wall_side;
        this.wall_axis = wall_axis;
    }

    public Type GetSideType()
    {
        if (wall_axis == WallObject.WallFacing.X_Axis && wall_side == WallObject.Side.A_Side)
        {
            return Type.xa;
        }
        else if (wall_axis == WallObject.WallFacing.X_Axis && wall_side == WallObject.Side.B_Side)
        {
            return Type.xb;
        }
        else if (wall_axis == WallObject.WallFacing.Y_Axis && wall_side == WallObject.Side.A_Side)
        {
            return Type.ya;
        }
        else if (wall_axis == WallObject.WallFacing.Y_Axis && wall_side == WallObject.Side.B_Side)
        {
            return Type.yb;
        }

        return Type.none;
    }


    public Vector2Int PrevOffset(Vector2Int offset)
    {
        return pos + offset;
    }
    public Vector2Int PrevOffset(int x, int y)
    {
        return pos + new Vector2Int(x,y);
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
        return pos + new Vector2Int(1, 0);
    }
    public Vector2Int NextY()
    {
        return pos + new Vector2Int(0, 1);
    }

    public Vector2Int CornerUpXY()
    {
        return pos + new Vector2Int(1, 1);
    }

    public bool CompareIsSimilar(WallSelection other)
    {
        if (pos == other.pos && wall_axis == other.wall_axis && wall_side == other.wall_side) return true;

        return false;
    }

    //Wall RULES
    //αXa -> αYb = ALWAYS ALLOWED
    //αXa -> αXb = Only if (Next_X() is null) && (+1,-1) has no y wall
    //αYa -> αYb = Only if (Next_Y() is null) && (-1,+1) has no x wall
    //αYa -> αXb = Only if (Prev_X() has no x wall && Prev_Y() as no y wall

    //αYa RULES
    //αYa -> βYa = Only if (β.Prev_X() has no x wall)

    //αYb RULES
    //αYb -> βYb = Only if β has no x wall 
    //αYb -> βXb = ALWAYS ALLOWED

    //αXa RULES
    //αXa -> βXa = Only if (β has no y wall)

    //αXb RULES
    //αXb -> βXb = Only if β has no x wall 
    //αXb -> βYb = ALWAYS ALLOWED

}

public class BuildMode_Wallpaper : BuildToolScript
{

    [ReadOnly] [SerializeField] private bool wallDetected = false;
    [ReadOnly] [SerializeField] private SK_Texture currentWallpaper;
    public GameObject debug_wallpaperTestA;
    public GameObject debug_wallpaperTestB;
    public Material defaultWallMaterial;
    private List<BuildData.WallData> previewWalldatas = new List<BuildData.WallData>();
    private bool b = false;
    private WallObject previousWallObj;

    private void OnEnable()
    {
    }



    public void SetWallpaper(SK_Texture texture)
    {
        BuildMode.Wallpaper.currentWallpaper = texture;
    }

    public override void RunTool()
    {
        wallDetected = false;

        if (currentWallpaper.isWallpaper == true)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50f))
            {
                var wallObject = hit.collider.gameObject.GetComponent<WallObject>();

                if (wallObject != null)
                {
                    if (b == false)
                    {
                        b = true;
                    }

                    wallDetected = true;
                    if (Input.GetMouseButton(0))
                    {
                        HoveringWallpaper(wallObject, BuildMode.CurrentFloorPlan.allWallDatas);
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            HoveringTravellingWallpaper(wallObject);
                        }
                        else
                        {
                            HoveringWallpaper(wallObject);
                        }
                    }
                }

            }
        }

        if (wallDetected == false)
        {
            if (b == true && Input.GetMouseButton(0) == false)
            {
                previewHighlightPrefabs.DestroyAndClearList_1();
                previewWalldatas = Lot.MyLot.floorplanData[Shopkeeper.Game.currentLevel].allWallDatas.Clone();
                BuildMode.Wall.ReRenderWallpaperOnly(previewWalldatas);

                b = false;
            }
            previousWallObj = null;
            if (previewWalldatas.Count > 0) previewWalldatas.Clear();
            debug_wallpaperTestA.gameObject.SetActive(false);
            debug_wallpaperTestB.gameObject.SetActive(false);
        }
    }

    public static List<SK_Texture> GetAllWallpaperTextures()
    {
        List<SK_Texture> allTextures = new List<SK_Texture>();
        foreach (var meta in Shopkeeper.Database.loaded_TextureAsset)
        {
            if (meta.isWallpaper == false) continue;
            var texture = Shopkeeper.Database.Load_SKTexture(meta);
            allTextures.Add(meta);

        }

        return allTextures;
    }


    private List<GameObject> previewHighlightPrefabs = new List<GameObject>();
    [SerializeField] [ReadOnly] private List<WallSelection> _wallselections = new List<WallSelection>();

    public void HoveringTravellingWallpaper(WallObject originWallpaper)
    {
        bool is_a_side = false;

        if (previousWallObj == originWallpaper)
        {
            return;
        }

        previewHighlightPrefabs.DestroyAndClearList_1();
        previewWalldatas = Lot.MyLot.floorplanData[Shopkeeper.Game.currentLevel].allWallDatas.Clone();
        previousWallObj = originWallpaper;
        _wallselections.Clear();

        Transform parent = originWallpaper.transform.parent;
        Vector3 localPos = parent.InverseTransformPoint(Camera.main.transform.position);

        if (localPos.z > 0)
        {
            is_a_side = true;
        }

        var firstWall = new WallSelection(originWallpaper.wallData.pos, is_a_side == true ? WallObject.Side.A_Side : WallObject.Side.B_Side, originWallpaper.currentWallFacing);
        bool has_ClosedLoop = false;
        int attempt = 0;//2000 must be stopped.
        _wallselections.Add(firstWall);

        WallSelection currentWall = firstWall;

        while (has_ClosedLoop == false && attempt <= 99)
        {
            //available wall to select
            var traverseables = GetTraverseableWalls(currentWall);

            if (traverseables.Count == 0)
            {
                has_ClosedLoop = true;
                break;
            }

            WallSelection tempWall = null;

            foreach(var wall in traverseables)
            {
                if (_wallselections.Contains(wall)) continue;
                tempWall = wall;
                break;
            }

            if (tempWall == null)
            {
                has_ClosedLoop = true;
                break;
            }

            _wallselections.Add(tempWall);
            currentWall = tempWall;

            attempt++;
        }

        if (attempt >= 99) { Debug.Log("BROKEN! Function is not working!"); }

        foreach(var wallSelected in _wallselections)
        {
            GameObject prefab = debug_wallpaperTestB;
            if (wallSelected.wall_side == WallObject.Side.B_Side) prefab = debug_wallpaperTestA;

            Vector3 _rotation = Vector3.zero;
            var wallDebug = Instantiate(prefab, transform);
            wallDebug.gameObject.SetActive(true);
            if (wallSelected.wall_axis == WallObject.WallFacing.Y_Axis) _rotation = new Vector3(0, -90, 0);

            wallDebug.transform.position = new Vector3(wallSelected.pos.x, 0, wallSelected.pos.y);
            wallDebug.transform.eulerAngles = _rotation;

            previewHighlightPrefabs.Add(wallDebug);

        }

    }

    /// <summary>
    /// Bruh why not just paint on one side of the wall (Y-axis b-wall all the way) no need to tracing like a madman.
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    private List<WallSelection> GetTraverseableWalls(WallSelection origin)
    {
        List<WallSelection> traverseables = new List<WallSelection>();
        WallSelection.Type type = origin.GetSideType();


        if (type == WallSelection.Type.ya)
        {
            //available options
            //(0,-1) ya
            //(-1,1) xb
            //(0,1) ya
            //(0,1) xa
            //(-1,0) xa
            //(0,0) xb 

            WallSelection[] available = new WallSelection[6]
            {
                new WallSelection(origin.pos + new Vector2Int(0, -1), WallObject.Side.A_Side, WallObject.WallFacing.Y_Axis),
                new WallSelection(origin.pos + new Vector2Int(-1, 1), WallObject.Side.B_Side, WallObject.WallFacing.X_Axis),
                new WallSelection(origin.pos + new Vector2Int(0, 1), WallObject.Side.A_Side, WallObject.WallFacing.Y_Axis),
                new WallSelection(origin.pos + new Vector2Int(0, 1), WallObject.Side.A_Side, WallObject.WallFacing.X_Axis),
                new WallSelection(origin.pos + new Vector2Int(-1, 0), WallObject.Side.A_Side, WallObject.WallFacing.X_Axis),
                new WallSelection(origin.pos + new Vector2Int(0, 0), WallObject.Side.B_Side, WallObject.WallFacing.X_Axis)
            }
            ;

            available = available.ToList().FindAll(x => _wallselections.FindWallSelectionExists(x) == false).ToArray(); 

            foreach(var ws in available)
            {
                if (CanTraverse(origin, ws))
                    traverseables.Add(ws);
            }


            //foreach (var trav1 in traverseables) Debug.Log(trav1.pos);


        }
        if (type == WallSelection.Type.yb)
        {
            //available options
            //(0,-1) yb
            //(-1,1) xa
            //(0,1) yb
            //(0,1) xb
            //(-1,0) xb
            //(0,0) xa 

            WallSelection[] available = new WallSelection[6]
             {
                    new WallSelection(origin.pos + new Vector2Int(0, -1), WallObject.Side.B_Side, WallObject.WallFacing.Y_Axis),
                    new WallSelection(origin.pos + new Vector2Int(-1, 1), WallObject.Side.A_Side, WallObject.WallFacing.X_Axis),
                    new WallSelection(origin.pos + new Vector2Int(0, 1), WallObject.Side.B_Side, WallObject.WallFacing.Y_Axis),
                    new WallSelection(origin.pos + new Vector2Int(0, 1), WallObject.Side.B_Side, WallObject.WallFacing.X_Axis),
                    new WallSelection(origin.pos + new Vector2Int(-1, 0), WallObject.Side.B_Side, WallObject.WallFacing.X_Axis),
                    new WallSelection(origin.pos + new Vector2Int(0, 0), WallObject.Side.A_Side, WallObject.WallFacing.X_Axis)
             }
             ;

            available = available.ToList().FindAll(x => _wallselections.FindWallSelectionExists(x) == false).ToArray();

            foreach (var ws in available)
            {
                if (CanTraverse(origin, ws))
                    traverseables.Add(ws);
            }
        }
        if (type == WallSelection.Type.xa)
        {
            //available options
            //(-1,0) xa
            //(1,0) xa
            //(0,0) yb
            //(1,0) ya
            //(0,-1) ya
            //(1,-1) yb
            WallSelection[] available = new WallSelection[6]
        {
                new WallSelection(origin.pos + new Vector2Int(-1, -0), WallObject.Side.A_Side, WallObject.WallFacing.X_Axis),
                new WallSelection(origin.pos + new Vector2Int(1, 0), WallObject.Side.A_Side, WallObject.WallFacing.X_Axis),
                new WallSelection(origin.pos + new Vector2Int(0, 0), WallObject.Side.B_Side, WallObject.WallFacing.Y_Axis),
                new WallSelection(origin.pos + new Vector2Int(1, 0), WallObject.Side.A_Side, WallObject.WallFacing.Y_Axis),
                new WallSelection(origin.pos + new Vector2Int(0, -1), WallObject.Side.A_Side, WallObject.WallFacing.Y_Axis),
                new WallSelection(origin.pos + new Vector2Int(1, -1), WallObject.Side.B_Side, WallObject.WallFacing.Y_Axis)
        }
        ;

            available = available.ToList().FindAll(x => _wallselections.FindWallSelectionExists(x) == false).ToArray();

            foreach (var ws in available)
            {
                if (CanTraverse(origin, ws))
                    traverseables.Add(ws);
            }

        }
        if (type == WallSelection.Type.xb)
        {
            //available options
            //(-1,0) xb
            //(1,0) xb
            //(0,0) ya
            //(1,0) yb
            //(0,-1) yb
            //(1,-1) ya

            WallSelection[] available = new WallSelection[6]
           {
                new WallSelection(origin.pos + new Vector2Int(-1, -0), WallObject.Side.B_Side, WallObject.WallFacing.X_Axis),
                new WallSelection(origin.pos + new Vector2Int(1, 0), WallObject.Side.B_Side, WallObject.WallFacing.X_Axis),
                new WallSelection(origin.pos + new Vector2Int(0, 0), WallObject.Side.A_Side, WallObject.WallFacing.Y_Axis),
                new WallSelection(origin.pos + new Vector2Int(1, 0), WallObject.Side.B_Side, WallObject.WallFacing.Y_Axis),
                new WallSelection(origin.pos + new Vector2Int(0, -1), WallObject.Side.B_Side, WallObject.WallFacing.Y_Axis),
                new WallSelection(origin.pos + new Vector2Int(1, -1), WallObject.Side.A_Side, WallObject.WallFacing.Y_Axis)
           }
           ;

            available = available.ToList().FindAll(x => _wallselections.FindWallSelectionExists(x) == false).ToArray();

            foreach (var ws in available)
            {
                if (CanTraverse(origin, ws))
                    traverseables.Add(ws);
            }
        }


        return traverseables;
    }


    /// <summary>
    /// e
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool CanTraverse(WallSelection origin, WallSelection target)
    {
        
        if (origin.GetSideType() == WallSelection.Type.ya)
        {
   
            if (target.GetSideType() == WallSelection.Type.ya && target.pos == origin.NextY())
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(target.pos);

                if (wallMain != null && IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    if (wallMain.y_wall && IsWallConnectedToThis(origin.NextY(), origin.PrevOffset(-1, 1)) == false)
                    {
                        return true;
                    }
                }  
            }

            if (target.GetSideType() == WallSelection.Type.ya && target.pos == origin.PrevY())
            {
                if (IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    if (IsWallConnectedToThis(origin.pos, origin.PrevX()) == false)
                    {
                        return true;
                    }
                }  
            }

            if (target.GetSideType() == WallSelection.Type.xb && target.pos == origin.PrevOffset(-1,1))
            {
                if (IsWallConnectedToThis(origin.NextY(), target.pos) == true)
                {
                    return true;
                }
            }

            if (target.GetSideType() == WallSelection.Type.xb && target.pos == origin.PrevOffset(0, 0))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(origin.pos);

                if (wallMain != null)
                {
                    if (wallMain.x_wall && IsWallConnectedToThis(origin.pos, origin.PrevX()) == false && IsWallConnectedToThis(origin.pos, origin.PrevY()) == false)
                        return true;
                }
            }

            if (target.GetSideType() == WallSelection.Type.xa && target.pos == origin.PrevX())
            {
                if (IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    return true;
                }
            }

            if (target.GetSideType() == WallSelection.Type.xa && target.pos == origin.PrevOffset(0, 1))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(target.pos);

                if (wallMain != null)
                {
                    if (wallMain.x_wall == true && wallMain.y_wall == false && IsWallConnectedToThis(origin.NextY(), origin.PrevOffset(-1,1)) == false)
                        return true;
                }
            }

        }

        if (origin.GetSideType() == WallSelection.Type.xb)
        {
            if (target.GetSideType() == WallSelection.Type.xb && target.pos == origin.NextX())
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(target.pos);

                if (wallMain != null && IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    if (wallMain.x_wall && IsWallConnectedToThis(origin.NextX(), origin.PrevOffset(1, -1)) == false)
                    {
                        return true;
                    }
                }
            }

            if (target.GetSideType() == WallSelection.Type.xb && target.pos == origin.PrevX())
            {
                if (IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    if (IsWallConnectedToThis(origin.pos, origin.PrevY()) == false)
                    {
                        return true;
                    }
                }
            }

            if (target.GetSideType() == WallSelection.Type.yb && target.pos == origin.PrevOffset(1, 0))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(target.pos);

                if (wallMain != null)
                {
                    if (wallMain.y_wall && wallMain.x_wall == false && IsWallConnectedToThis(origin.NextX(), origin.PrevOffset(1, -1)) == false)
                        return true;
                }
            }

            if (target.GetSideType() == WallSelection.Type.yb && target.pos == origin.PrevOffset(0, -1))
            {
                if (IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    return true;
                }
            }

            if (target.GetSideType() == WallSelection.Type.ya && target.pos == origin.PrevOffset(1, -1))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(origin.PrevOffset(1,-1));

                if (wallMain != null)
                {
                    if (wallMain.y_wall && IsWallConnectedToThis(origin.pos, origin.NextX()) == true)
                    {
                        return true;
                    }
                }
            }

            if (target.GetSideType() == WallSelection.Type.ya && target.pos == origin.PrevOffset(0, 0))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(origin.pos);

                if (wallMain != null)
                {
                    if (wallMain.y_wall && IsWallConnectedToThis(origin.pos, origin.PrevX()) == false && IsWallConnectedToThis(origin.pos, origin.PrevY()) == false)
                        return true;
                }
            }

        }

        if (origin.GetSideType() == WallSelection.Type.yb)
        {

            if (target.GetSideType() == WallSelection.Type.yb && target.pos == origin.NextY())
            {
                if (IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    if (IsWallConnectedToThis(origin.NextY(), origin.PrevOffset(1, 1)) == false)
                    {
                        return true;
                    }
                }
            }

            if (target.GetSideType() == WallSelection.Type.yb && target.pos == origin.PrevY())
            {
                if (IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    if (IsWallConnectedToThis(origin.pos, origin.NextX()) == false)
                    {
                        return true;
                    }
                }
            }

            if (target.GetSideType() == WallSelection.Type.xa && target.pos == origin.PrevOffset(-1, 1))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(target.pos);
                var wallMain_1 = previewWalldatas.IsWallDataExistAt(origin.NextY());

                if (wallMain != null)
                {
                    if (wallMain_1 != null)
                    {
                        if (wallMain.x_wall && wallMain_1.x_wall == false && wallMain_1.y_wall == false)
                            return true;
                    }
                    else if (wallMain.x_wall)
                    {
                        return true;
                    }

        
                }
            }

            if (target.GetSideType() == WallSelection.Type.xa && target.pos == origin.PrevOffset(0, 0))
            {
                if (IsWallConnectedToThis(origin.pos, origin.NextX()) == true)
                {
                    return true;
                }
            }

            if (target.GetSideType() == WallSelection.Type.xb && target.pos == origin.PrevOffset(-1,0))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(target.pos);

                if (wallMain != null)
                {
                    if (wallMain.x_wall == true && IsWallConnectedToThis(origin.pos, origin.NextX()) == false && IsWallConnectedToThis(origin.pos, origin.PrevOffset(0, -1)) == false)
                        return true;
                }
               
            }

            if (target.GetSideType() == WallSelection.Type.xb && target.pos == origin.PrevOffset(0, 1))
            {
                if (IsWallConnectedToThis(origin.NextY(), origin.PrevOffset(1,1)) == true)
                {
                    return true;
                }
            }

        }

        if (origin.GetSideType() == WallSelection.Type.xa)
        {
            if (target.GetSideType() == WallSelection.Type.xa && target.pos == origin.NextX())
            {
                var nextwall = previewWalldatas.IsWallDataExistAt(target.pos);

                if (nextwall != null && IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    if (nextwall.x_wall && nextwall.y_wall == false)
                    {
                        return true;
                    }
                }
            }

            if (target.GetSideType() == WallSelection.Type.xa && target.pos == origin.PrevX())
            {
                var originWall = previewWalldatas.IsWallDataExistAt(origin.pos);

                if (originWall != null && IsWallConnectedToThis(origin.pos, target.pos) == true)
                {
                    if (originWall.y_wall == false && originWall.x_wall)
                    {
                        return true;
                    }
                }
            }

            if (target.GetSideType() == WallSelection.Type.ya && target.pos == origin.PrevOffset(1, 0))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(origin.NextX());

                if (wallMain != null)
                {
                    if (wallMain.y_wall == true)
                    {
                        return true;
                    }
                }
            }

            if (target.GetSideType() == WallSelection.Type.yb && target.pos == origin.PrevOffset(0, 0))
            {
                if (IsWallConnectedToThis(origin.pos, origin.NextY()) == true)
                {
                    return true;
                }
            }

            if (target.GetSideType() == WallSelection.Type.yb && target.pos == origin.PrevOffset(1, -1))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(origin.NextX());
                var wallMain_1 = previewWalldatas.IsWallDataExistAt(origin.PrevOffset(1, -1));

                if (wallMain != null)
                {
                    if (IsWallConnectedToThis(wallMain.pos, target.pos) == true && wallMain.x_wall == false && wallMain.y_wall == false)
                        return true;
                }
                if (wallMain_1 != null)
                {
                    if (wallMain == null && wallMain_1.y_wall)
                        return true;

                    if (wallMain != null && wallMain_1.y_wall)
                    {
                        if (IsWallConnectedToThis(wallMain.pos, target.pos) == true && wallMain.x_wall == false && wallMain.y_wall == false)
                            return true;
                    }
                }
            }

            if (target.GetSideType() == WallSelection.Type.ya && target.pos == origin.PrevOffset(0, -1))
            {
                var wallMain = previewWalldatas.IsWallDataExistAt(origin.pos);

                if (wallMain != null)
                {
                    if (IsWallConnectedToThis(origin.pos, origin.PrevX()) == false && IsWallConnectedToThis(origin.pos, origin.PrevY()) == true && wallMain.y_wall == false)
                        return true;
                }
            }

        }

        {
            //Bad organization
            //if (origin.pos == target.pos)
            //{
            //    //xa -> yb always possible
            //    if (origin.GetSideType() == WallSelection.Type.xa && target.GetSideType() == WallSelection.Type.yb)
            //    {
            //        return true;
            //    }
            //    if (origin.GetSideType() == WallSelection.Type.xa && target.GetSideType() == WallSelection.Type.xb)
            //    {

            //        var wall_custom = previewWalldatas.IsWallDataExistAt(origin.PrevOffset(new Vector2Int(1, -1)));

            //        if (previewWalldatas.IsWallDataExistAt(origin.NextX()) == null)
            //        {
            //            if (wall_custom != null)
            //            {
            //                if (wall_custom.y_wall == false)
            //                {
            //                    return true;
            //                }
            //            }
            //            else
            //            {
            //                return true;
            //            }
            //        }
            //    }
            //    if (origin.GetSideType() == WallSelection.Type.ya && target.GetSideType() == WallSelection.Type.yb)
            //    {
            //        var wall_custom = previewWalldatas.IsWallDataExistAt(origin.PrevOffset(new Vector2Int(-1, 1)));

            //        if (previewWalldatas.IsWallDataExistAt(origin.NextY()) == null)
            //        {
            //            if (wall_custom != null)
            //            {
            //                if (wall_custom.x_wall == false)
            //                {
            //                    return true;
            //                }
            //            }
            //            else
            //            {
            //                return true;
            //            }
            //        }
            //    }
            //    if (origin.GetSideType() == WallSelection.Type.ya && target.GetSideType() == WallSelection.Type.xb)
            //    {
            //        bool b1 = IsWallConnectedToThis(origin.pos, origin.PrevX());
            //        bool b2 = IsWallConnectedToThis(origin.pos, origin.PrevY());

            //        if (b1 == false && b2 == false) return true;
            //    }
            //}
            //if (target.pos == origin.NextX()) //(1,0)
            //{
            //    if (origin.GetSideType() == WallSelection.Type.ya)
            //    {
            //        if ()
            //}
            //}
        }

        return false;
    }

    private bool IsWallConnectedToThis(Vector2Int pos, Vector2Int target)
    {
        var wallData = previewWalldatas.IsWallDataExistAt(pos);
        var targetWallData = previewWalldatas.IsWallDataExistAt(target);

        if (wallData == null)
        {
            if (targetWallData != null)
            {
                if (targetWallData.x_wall && pos == target + new Vector2Int(1,0))
                {
                    return true;
                }
            }

            return false;
        }
        if (targetWallData == null)
        {
            if (wallData != null)
            {
                if (wallData.x_wall && target == pos + new Vector2Int(1, 0))
                {
                    return true;
                }
            }

            return false;
        }

        var next_X = wallData.NextX();
        var next_Y = wallData.NextY();
        var prev_X = wallData.PrevX();
        var prev_Y = wallData.PrevY();

        var prev_X_WallData = previewWalldatas.IsWallDataExistAt(prev_X);
        var prev_Y_WallData = previewWalldatas.IsWallDataExistAt(prev_Y);


        if (wallData.x_wall && target == next_X)
        {
            return true;
        }
        if (wallData.y_wall && target == next_Y)
        {
            return true;
        }


        if (prev_X_WallData != null)
        {
            //Debug.Log($"{pos} | {target}");

            if (prev_X_WallData.x_wall && target == prev_X)
            {
                return true;
            }
        }

        if (prev_Y_WallData != null)
        {
            if (prev_Y_WallData.y_wall && target == prev_Y)
            {
                return true;
            }
        }

        return false;

    }




    public void HoveringWallpaper(WallObject wallObject, List<BuildData.WallData> toOverride = null)
    {
        bool is_a_side = false; //check camera direction

        if (previousWallObj == wallObject && toOverride == null)
        {
            //status quo
            return;
        }

        previewWalldatas = Lot.MyLot.floorplanData[Shopkeeper.Game.currentLevel].allWallDatas.Clone();

        previousWallObj = wallObject;


        Transform parent = wallObject.transform.parent;

        Vector3 localPos = parent.InverseTransformPoint(Camera.main.transform.position);
        GameObject targetedObject = null;

        if (localPos.z > 0)
        {
            debug_wallpaperTestA.gameObject.SetActive(false);
            debug_wallpaperTestB.gameObject.SetActive(true);
            is_a_side = true;

            targetedObject = debug_wallpaperTestB;
        }
        else
        {
            debug_wallpaperTestA.gameObject.SetActive(true);
            debug_wallpaperTestB.gameObject.SetActive(false);

            targetedObject = debug_wallpaperTestA;
        }

        if (targetedObject != null)
        {
            targetedObject.transform.position = parent.transform.position;
            targetedObject.transform.eulerAngles = parent.transform.eulerAngles;
        }

        var wallDataToChange = previewWalldatas.IsWallDataExistAt(wallObject.wallData.pos);


        if (toOverride != null)
        {
            wallDataToChange = toOverride.IsWallDataExistAt(wallObject.wallData.pos);
            //Debug.Log($"{wallDataToChange} some wall changed");
        }

        if (wallObject.currentWallFacing == WallObject.WallFacing.X_Axis)
        {
            if (is_a_side)
            {
                wallDataToChange.wallpaperAssetPath_x_aSide = currentWallpaper.filePath_MainTexture;
            }
            else
            {
                wallDataToChange.wallpaperAssetPath_x_bSide = currentWallpaper.filePath_MainTexture;
            }
        }
        else
        {
            if (is_a_side)
            {
                wallDataToChange.wallpaperAssetPath_y_aSide = currentWallpaper.filePath_MainTexture;
            }
            else
            {
                wallDataToChange.wallpaperAssetPath_y_bSide = currentWallpaper.filePath_MainTexture;
            }
        }

        if (toOverride == null)
        {
            BuildMode.Wall.ReRenderWallpaperOnly(previewWalldatas);
        }
        else
        {
            BuildMode.Wall.ReRenderWallpaperOnly(toOverride);
        }
    }

    public void PaintWall(WallObject wallObject)
    {
        //0 no side
        //1 b side
        //2 a side
        var skTexture_a = Shopkeeper.Database.Get_SKTexture(wallObject.currentWallFacing == WallObject.WallFacing.X_Axis ?  wallObject.wallData.wallpaperAssetPath_x_aSide : wallObject.wallData.wallpaperAssetPath_y_aSide);
        var skTexture_b = Shopkeeper.Database.Get_SKTexture(wallObject.currentWallFacing == WallObject.WallFacing.X_Axis ? wallObject.wallData.wallpaperAssetPath_x_bSide : wallObject.wallData.wallpaperAssetPath_y_bSide);
        Material material_a = null; 
        Material material_b = null; 
        if (skTexture_a != null) { material_a = Shopkeeper.Database.Load_Material(skTexture_a); }
        if (skTexture_b != null) { material_b = Shopkeeper.Database.Load_Material(skTexture_b); }

        Material[] mats = wallObject.meshRndrWall.materials;

        if (material_b != null)
        {
            mats[1] = material_b;
        }
        else
        {
            mats[1] = defaultWallMaterial;
        }

        if (material_a != null)
        {
            mats[2] = material_a;
        }
        else
        {
            mats[2] = defaultWallMaterial;
        }

        wallObject.meshRndrWall.materials = mats;
    }

    public void PaintExtraWall(WallObject wallObject, List<BuildData.WallData> allWallDots = null)
    {
        //0 x_a side
        //1 y_b side
        //2 x_b side
        //3 no side
        //4 y_a side
        var skTexture_x_a = Shopkeeper.Database.Get_SKTexture(wallObject.wallData.wallpaperAssetPath_x_aSide);
        var skTexture_x_b = Shopkeeper.Database.Get_SKTexture(wallObject.wallData.wallpaperAssetPath_x_bSide);
        var skTexture_y_a = Shopkeeper.Database.Get_SKTexture(wallObject.wallData.wallpaperAssetPath_y_aSide);
        var skTexture_y_b = Shopkeeper.Database.Get_SKTexture(wallObject.wallData.wallpaperAssetPath_y_bSide);

        if (wallObject.cornerXY_wall == true && allWallDots != null)
        {
            var prevX_wall = allWallDots.IsWallDataExistAt(wallObject.wallData.PrevOffset(new Vector2Int(-1,1)));

            if (prevX_wall != null)
            {
                skTexture_x_a = Shopkeeper.Database.Get_SKTexture(prevX_wall.wallpaperAssetPath_x_aSide);
                skTexture_x_b = Shopkeeper.Database.Get_SKTexture(prevX_wall.wallpaperAssetPath_x_bSide);
            }
        }

        var cornerType = BuildMode.Wall.IsExtraWall_a_Corner(wallObject, allWallDots);

        if (cornerType != BuildMode_Wall.WallCornerType.none && wallObject.cornerXY_wall == false)
        {
            wallObject.cornerType = cornerType;

            if (cornerType == BuildMode_Wall.WallCornerType._xy)
            {
                var prevX_wall = allWallDots.IsWallDataExistAt(wallObject.wallData.PrevX());

                if (prevX_wall != null)
                {
                    skTexture_x_a = Shopkeeper.Database.Get_SKTexture(prevX_wall.wallpaperAssetPath_x_aSide);
                    skTexture_x_b = Shopkeeper.Database.Get_SKTexture(prevX_wall.wallpaperAssetPath_x_bSide);
                }
            }

            if (cornerType == BuildMode_Wall.WallCornerType.x_y)
            {
                var prevY_wall = allWallDots.IsWallDataExistAt(wallObject.wallData.PrevY());

                if (prevY_wall != null)
                {
                    skTexture_y_a = Shopkeeper.Database.Get_SKTexture(prevY_wall.wallpaperAssetPath_y_aSide);
                    skTexture_y_b = Shopkeeper.Database.Get_SKTexture(prevY_wall.wallpaperAssetPath_y_bSide);
                }
            }
        }


        Material material_x_a = null;
        Material material_x_b = null;
        Material material_y_a = null;
        Material material_y_b = null;

        if (skTexture_x_a != null) { material_x_a = Shopkeeper.Database.Load_Material(skTexture_x_a); }
        if (skTexture_x_b != null) { material_x_b = Shopkeeper.Database.Load_Material(skTexture_x_b); }
        if (skTexture_y_a != null) { material_y_a = Shopkeeper.Database.Load_Material(skTexture_y_a); }
        if (skTexture_y_b != null) { material_y_b = Shopkeeper.Database.Load_Material(skTexture_y_b); }

        Material[] mats = wallObject.meshRndrWall.materials;

        mats[0] = AssignMaterial(material_x_a);
        mats[1] = AssignMaterial(material_y_b);
        mats[2] = AssignMaterial(material_x_b);
        mats[4] = AssignMaterial(material_y_a);




        wallObject.meshRndrWall.materials = mats;
    }

    public Material AssignMaterial(Material mat)
    {
        if (mat == null) return defaultWallMaterial;

        return mat;
    }

}
