using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

public static class ShopkeeperUtilities
{

    public static double ConvertToUnixTimestamp(this DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return Math.Floor(diff.TotalSeconds);
    }

    public static void EnableGameobject(this GameObject go, bool active)
    {
        if (go.activeSelf != active) go.SetActive(active);
    }
    public static bool IsParentOf(this GameObject origin, GameObject parent)
    {
        Transform t = origin.transform;
        if (t == parent.transform) return true;

        while (t.parent != null)
        {
            t = t.parent;

            if (t == parent.transform) return true;
        }


        return false;
    }

    public static BuildData.WallData IsWallDataExistAt(this List<BuildData.WallData> list, Vector2Int pos)
    {
        var wallDat = list.Find(x => x.pos.x == pos.x && x.pos.y == pos.y);

        if (wallDat != null)
        {
            if (wallDat.x_wall | wallDat.y_wall)
            {
                return wallDat;
            }
        }

        return null;
    }


    public static bool FindWallSelectionExists(this List<WallSelection> list, WallSelection target)
    {
        foreach(var item in list)
        {
            if (item.CompareIsSimilar(target))
            {
                return true;
            }
        }

        return false;
    }



    public static void RemoveAllEmptyWalls(this List<BuildData.WallData> list)
    {
        foreach (var wall in list.Clone())
        {
            list.RemoveAll(x => x.x_wall == false && x.y_wall == false);
        }

    }

    public static void DestroyAndClearList<T>(this List<T> list) where T : Component
    {
        foreach (var wall in list)
        {
            if (wall == null) continue;
            UnityEngine.Object.Destroy(wall.gameObject);
        }

        list.Clear();
    }



    public static void DestroyAndClearList_1(this List<GameObject> list)
    {
        foreach (var wall in list)
        {
            if (wall == null) continue;
            UnityEngine.Object.Destroy(wall.gameObject);
        }

        list.Clear();
    }


    /// <summary>
    /// Check if direction is only either Left, Right, Down, Up, Forward or Back.
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    public static bool OnlySingleDirection(this Vector3 v3)
    {
        if (Mathf.Abs(v3.x) > 0.1f)
        {
            if (Mathf.Abs(v3.y) <= float.Epsilon && Mathf.Abs(v3.z) <= float.Epsilon)
            {
                return true;
            }
        }
        if (Mathf.Abs(v3.y) > 0.1f)
        {
            if (Mathf.Abs(v3.x) <= float.Epsilon && Mathf.Abs(v3.z) <= float.Epsilon)
            {
                return true;
            }
        }
        if (Mathf.Abs(v3.z) > 0.1f)
        {
            if (Mathf.Abs(v3.x) <= float.Epsilon && Mathf.Abs(v3.y) <= float.Epsilon)
            {
                return true;
            }
        }

        return false;
    }

    public static T Clone<T>(this T source)
    {
        var serialized = JsonConvert.SerializeObject(source, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        return JsonConvert.DeserializeObject<T>(serialized);
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static T GetComponentInParent<T>(this GameObject origin)
    {
        Transform t = origin.transform;

        while (t.parent != null)
        {
            t = t.parent;

            T component = t.gameObject.GetComponent<T>();

            if (component != null) return component;
        }


        return default(T);
    }

    public static Transform[] GetAllParents(this GameObject origin)
    {
        List<Transform> parents = new List<Transform>();
        Transform t = origin.transform;

        while (t.parent != null)
        {
            t = t.parent;

            parents.Add(t);
        }


        return parents.ToArray();
    }


    public static T GetComponentThenParent<T>(this GameObject origin)
    {
        T component = origin.GetComponent<T>();

        if (component != null) return component;
        else component = origin.GetComponentInParent<T>();

        if (component != null) return component;

        return default(T);
    }



    public static T GetComponentThenChild<T>(this GameObject origin)
    {
        T component = origin.GetComponent<T>();

        if (component != null) return component;
        else component = origin.GetComponentInChildren<T>();

        if (component != null) return component;

        return default(T);
    }


    public static List<GameObject> AllChilds(this GameObject root)
    {
        List<GameObject> result = new List<GameObject>();
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(result, VARIABLE.gameObject);
            }
        }
        return result;
    }
    private static void Searcher(List<GameObject> list, GameObject root)
    {
        list.Add(root);
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(list, VARIABLE.gameObject);
            }
        }
    }

    public static void DisableObjectTimer(this GameObject gameObject, float time = 5f)
    {
        var deactivator = gameObject.GetComponent<TimedObjectDeactivator>();
        if (deactivator == null) deactivator = gameObject.AddComponent<TimedObjectDeactivator>();
        deactivator.allowRestart = true;
        deactivator.Timer = time;
    }
}
