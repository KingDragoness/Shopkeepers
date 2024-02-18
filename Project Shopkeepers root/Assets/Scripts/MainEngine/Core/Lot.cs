using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Lot : MonoBehaviour
{

    public LotData currentLot;
    public LineRenderer lineBarrier;
    public LineRenderer emptyLine;

    public static readonly int CameraCeilingMin = 2;
    public static readonly int CameraCeilingMax = 50;
    public static readonly float CameraExtraAreaMult = 1.33f;

    private List<LineRenderer> allLines = new List<LineRenderer>();

    public static Vector3 CenterPivot()
    {
        return new Vector3(0, 0, Shopkeeper.Lot.currentLot.lotSize.y/2f);
    }

    public static LotData MyLot
    {
        get { return Shopkeeper.Lot.currentLot; }
    }

    private void Start()
    {
        Vector3[] lines = new Vector3[4];
        lines[0] = new Vector3(-currentLot.lotSize.x / 2f, 0.1f, 0f);
        lines[1] = new Vector3(-currentLot.lotSize.x / 2f, 0.1f, currentLot.lotSize.y);
        lines[2] = new Vector3(currentLot.lotSize.x / 2f, 0.1f, currentLot.lotSize.y);
        lines[3] = new Vector3(currentLot.lotSize.x / 2f, 0.1f, 0f);

        lineBarrier.SetPositions(lines);

        float halfX = currentLot.lotSize.x/2f;
        float halfY = currentLot.lotSize.y/2f;

        for(int x = 0; x < currentLot.lotSize.x; x++)
        {

            {
                Vector3 start = new Vector3(x, 0.03f, 0f);
                start.x -= halfX;

                Vector3 end = new Vector3(x, 0.03f, currentLot.lotSize.y);
                end.x -= halfX;

                Vector3[] v3Line = new Vector3[2];
                v3Line[0] = start;
                v3Line[1] = end;
                var line1 = Instantiate(emptyLine, transform);

                {
                    Gradient gradient = new Gradient();
                    gradient.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(0.75f, 0.0f), new GradientAlphaKey(0.75f, 1.0f) }
                    );
                    line1.colorGradient = gradient;
                }
               
                line1.gameObject.name = "HorizontalLine";
                line1.SetPositions(v3Line);
            }

            for (int y = 0; y < currentLot.lotSize.y; y++)
            {
                Vector3 start = new Vector3(x, 0.03f, y);
                start.x -= halfX;

                Vector3 end = new Vector3(halfX, 0.03f, y);

                Vector3[] v3Line = new Vector3[2];
                v3Line[0] = start;
                v3Line[1] = end;
                var line1 = Instantiate(emptyLine, transform);
                line1.SetPositions(v3Line);
            }
        }

    }

    public void InitializeLotData()
    {
        if (currentLot.floorplanData.Count == 0)
        {
            currentLot.floorplanData.Add(new BuildData());
        }
    }
}
