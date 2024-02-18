using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[Serializable]
public abstract class CC_Base
{
    public abstract string CommandName { get; }

    public abstract void ExecuteCommand(string[] args);
}

public class CC_OpenAssetCreator : CC_Base
{
    public override string CommandName { get { return "assetcreator"; } }

    public override void ExecuteCommand(string[] args)
    {
        AssetCreatorUI.Toggle();
    }
}