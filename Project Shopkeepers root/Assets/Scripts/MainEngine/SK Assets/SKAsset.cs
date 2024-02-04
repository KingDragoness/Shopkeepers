using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SKAsset
{
    /// <summary>
    /// PREFIXES
    /// b_ = base item
    /// d_ = dependent from other data
    /// </summary>

    //public static readonly string Extension = ".skitem";

    public string fileName = "UntitledName";
    public abstract string Extension { get; }


}
