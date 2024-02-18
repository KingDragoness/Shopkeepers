using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class BuildMode_Wall : BuildToolScript
{

    [FoldoutGroup("DEBUG")] public GameObject debug_wallPillar;
    [FoldoutGroup("DEBUG")] public LineRenderer debug_line;

    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_Simple;
    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_ExtraL;
    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_ExtraR;
    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_2DotCorner;
    [FoldoutGroup("Prefabs")] public GameObject prefab_Wall_Triangle;

    private Vector3Int startingWallPoint = new Vector3Int();
    private BuildData previewFloorPlan;
    [SerializeField] [ReadOnly] private int distance = 1;
    [SerializeField] [ReadOnly] private bool isInvalidDraw = false;
    [SerializeField] [ReadOnly] private bool isDeleteMode = false;
    private Vector3Int[] newDots = new Vector3Int[0];


    public BuildData lotDataCurrent
    {
        get { return Lot.MyLot.floorplanData[Shopkeeper.Game.currentLevel]; }
    }

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
            previewFloorPlan = Lot.MyLot.floorplanData[Shopkeeper.Game.currentLevel].Clone();
            newDots = new Vector3Int[0];

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (newDots.Length > 0)
            {
                CreateNewWalls(isDeleteMode);
                Debug.Log("Draw walls into the lot");
            }

            newDots = new Vector3Int[0];
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 currentArrowPos = Shopkeeper.BuildMode.Arrow.GetArrowPosition();
            distance = Mathf.RoundToInt((currentArrowPos - startingWallPoint).magnitude);
            newDots = new Vector3Int[distance + 1];
            if (distance < 1) newDots = new Vector3Int[0];
            Vector3 dir = (currentArrowPos - startingWallPoint).normalized;

            if (dir.OnlySingleDirection() && distance >= 1)
            {
                isInvalidDraw = false;
                for (int x = 0; x <= distance; x++)
                {
                    Vector3 ffs = startingWallPoint + (x * dir);
                    newDots[x].x = Mathf.RoundToInt(ffs.x);
                    newDots[x].y = Mathf.RoundToInt(ffs.y);
                    newDots[x].z = Mathf.RoundToInt(ffs.z);

                }

            }
            else
            {
                isInvalidDraw = true;
            }

            if (isInvalidDraw)
            {
                newDots = new Vector3Int[0];
            }

            DrawNewDots(newDots, isDeleteMode);

            //paint dots on the map
            Shopkeeper.BuildMode.Arrow.SetArrowColorUsed();

        }
        else
        {
            Shopkeeper.BuildMode.Arrow.SetArrowColorIdle();

        }

    }

    private void OnGUI()
    {
        if (Shopkeeper.Game.DEBUG_ALLOW_DEBUG_GUI == false) return;

        GUI.Label(new Rect(10, 20, 100, 20), $"isInvalidDraw:{isInvalidDraw}");
    }

    public void DrawNewDots(Vector3Int[] allVector3, bool isDeleteMode = false)
    {
        List<BuildData.WallDot> tempWallDots = new List<BuildData.WallDot>();

        if (isDeleteMode == true)
        {
            tempWallDots.AddRange(previewFloorPlan.allWalls);
        }

        int length = allVector3.Length;
        int index_0 = 0;

        Vector2Int prevPosCell = new Vector2Int(-256,-256);

        foreach (var pos in allVector3)
        {
            Vector2Int posCell = new Vector2Int(pos.x, pos.z);

            if (isDeleteMode == false)
            {
                BuildData.WallDot wallDot = new BuildData.WallDot(new Vector2Int(-256, -256));

                if (AlreadyExistCell(previewFloorPlan.allWalls, posCell) == false)
                {
                    wallDot = new BuildData.WallDot(posCell);
                    tempWallDots.Add(wallDot);
                }
                else
                {
                    wallDot = GetCell(previewFloorPlan.allWalls, posCell);
                }

                //override the cell's connections
                if (prevPosCell.x > -256 && (index_0 + 1) <= length)
                {
                    wallDot.AddConnection(prevPosCell);
                }
            }
            else
            {
                if (AlreadyExistCell(previewFloorPlan.allWalls, posCell) == true)
                {
                    //this is we deleting "dot B"
                    var dotSimilar = tempWallDots.Find(t => t.pos.x == posCell.x && t.pos.y == posCell.y);

                    bool doNOTDeleteDot = false;

                    if (dotSimilar.connectedDots.Count > 0)
                    {
                        var toBeDeletedPoints = ConvertToVector2Int(allVector3.ToList());

                        foreach (var v2 in dotSimilar.connectedDots)
                        {
                            //If the "toBeDeletedPoints" contains this dot (dot C), ignore it, don't transfer.
                            if (toBeDeletedPoints.Exists(x => x == v2))
                            {

                            }
                            //If it doesn't, then we must transfer to the current dot (dot B is now child of dot C)
                            else
                            {
                                doNOTDeleteDot = true;
                            }
                        }
                    }

                    if (doNOTDeleteDot == false)
                    {
                        tempWallDots.RemoveAll(t => t.pos.x == posCell.x && t.pos.y == posCell.y);//postpone, don't delete first
                    }
                    

                }
            }

            index_0++;
            prevPosCell = posCell;
        }

        if (isDeleteMode == false)
        {
            tempWallDots.AddRange(previewFloorPlan.allWalls);
        }

        tempWallDots.ShareWallConnections();
        RenderWalls(tempWallDots);
    }

    public void CreateNewWalls(bool isDeleteMode = false)
    {
        int length = newDots.Length;
        int index_0 = 0;

        Vector2Int prevPosCell = new Vector2Int(-256, -256);

        foreach (var pos in newDots)
        {
            Vector2Int posCell = new Vector2Int(pos.x, pos.z);

            if (isDeleteMode == false)
            {
                //if (AlreadyExistCell(lotDataCurrent.allWalls, posCell) == false) lotDataCurrent.allWalls.Add(new BuildData.WallDot(posCell));

                BuildData.WallDot wallDot = new BuildData.WallDot(new Vector2Int(-256, -256));

                if (AlreadyExistCell(lotDataCurrent.allWalls, posCell) == false)
                {
                    wallDot = new BuildData.WallDot(posCell);
                    lotDataCurrent.allWalls.Add(wallDot);
                }
                else
                {
                    wallDot = GetCell(lotDataCurrent.allWalls, posCell);
                }

                //override the cell's connections
                if (prevPosCell.x > -256 && (index_0 + 1) <= length)
                {
                    wallDot.AddConnection(prevPosCell);
                    //Debug.Log($"add connection {wallDot.connectedDots.Count}");
                }
            }
            else
            {
                if (AlreadyExistCell(lotDataCurrent.allWalls, posCell) == true)
                {
                    lotDataCurrent.allWalls.RemoveAll(t => t.pos.x == posCell.x && t.pos.y == posCell.y);
                }
            }

            index_0++;
            prevPosCell = posCell;
        }

        lotDataCurrent.allWalls.ShareWallConnections();
        RenderWalls(lotDataCurrent.allWalls);
    }

    public List<Vector2Int> ConvertToVector2Int(List<Vector3Int> list)
    {
        List<Vector2Int> allVector2d = new List<Vector2Int>();

        foreach(var pos in list)
        {
            allVector2d.Add(new Vector2Int(pos.x, pos.z));
        }

        return allVector2d;
    }

    public bool AlreadyExistCell(List<BuildData.WallDot> allWallDots, Vector2Int pos)
    {

        foreach (var wallDot in allWallDots)
        {
            if (wallDot.pos.x == pos.x && wallDot.pos.y == pos.y)
            {
                return true;
            }
        }

        return false;
    }

    public BuildData.WallDot GetCell(List<BuildData.WallDot> allWallDots, Vector2Int pos)
    {

        foreach (var wallDot in allWallDots)
        {
            if (wallDot.pos.x == pos.x && wallDot.pos.y == pos.y)
            {
                return wallDot;
            }
        }

        return new BuildData.WallDot(new Vector2Int(-256,-256));
    }

    private List<GameObject> allWallTests = new List<GameObject>();
    private List<LineRenderer> allLineTests = new List<LineRenderer>();
    private List<Vector3> alreadyDrawnLines = new List<Vector3>();

    /// <summary>
    /// Generate walls in the game.
    /// </summary>
    public void RenderWalls(List<BuildData.WallDot> allWallDots)
    {

        allWallTests.DestroyAndClearList_1();
        allLineTests.DestroyAndClearList();
        alreadyDrawnLines.Clear();

        foreach (var wallDot in allWallDots)
        {
            var newWallprefab = Instantiate(debug_wallPillar, transform);
            Vector3 pos = new Vector3(wallDot.pos.x, 0f, wallDot.pos.y);
            newWallprefab.gameObject.SetActive(true);
            newWallprefab.transform.position = pos;
            allWallTests.Add(newWallprefab);

            //connections
            foreach(var dot1 in wallDot.connectedDots)
            {
                Vector3[] positions = new Vector3[2];
                positions[0] = new Vector3(wallDot.pos.x, 0.02f, wallDot.pos.y);
                positions[1] = new Vector3(dot1.x, 0.02f, dot1.y);

                if (alreadyDrawnLines.Exists(x => x == positions[0]) && alreadyDrawnLines.Exists(x => x == positions[1])) continue;

                var newWireLine = Instantiate(debug_line, transform);      

                newWireLine.gameObject.SetActive(true);
                newWireLine.SetPositions(positions);
                allLineTests.Add(newWireLine);
                alreadyDrawnLines.Add(positions[0]);
                alreadyDrawnLines.Add(positions[1]);

            }
        }
    }
}
