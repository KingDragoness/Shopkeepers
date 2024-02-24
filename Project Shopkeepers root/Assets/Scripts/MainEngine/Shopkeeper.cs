using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using Sirenix.OdinInspector;




public class Shopkeeper : MonoBehaviour
{

    [SerializeField] private Transform _3dSpace;
    [SerializeField] private ShopkeeperGame _game;
    [SerializeField] private DatabaseAssetHandler _database;
    [SerializeField] private AssetCreator _assetCreator;
    [SerializeField] private MainUI _UIDatabase;
    [SerializeField] private Lot _lot;
    [SerializeField] private BuildMode _buildMode;
    [SerializeField] private ConsoleCommands _consoleCommand;
    [SerializeField] private InternalGameAssetDatabase _internalAsset;

    public static Transform Scene
    {
        get
        {
            return Instance._3dSpace;
        }
    }

    public static ShopkeeperGame Game
    {
        get
        {
            return Instance._game;
        }
    }

    public static InternalGameAssetDatabase InternalAsset
    {
        get
        {
            return Instance._internalAsset;
        }
    }

    public static Lot Lot
    {
        get
        {
            return Instance._lot;
        }
    }
    public static BuildMode BuildMode
    {
        get
        {
            return Instance._buildMode;
        }
    }

    public static DatabaseAssetHandler Database
    {
        get
        {
            return Instance._database;
        }
    }
    public static AssetCreator AssetCreator
    {
        get
        {
            return Instance._assetCreator;
        }
    }

    public static MainUI UI
    {
        get
        {
            return Instance._UIDatabase;
        }
    }

    public static ConsoleCommands Console
    {
        get
        {
            return Instance._consoleCommand;
        }
    }


    public static Shopkeeper Instance;

    public static readonly string Path_SavePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Shopkeeper/Saves";
    public static readonly string Path_StreamingAssets = Application.streamingAssetsPath;


    private void Awake()
    {
        Instance = this;
        BuildFolders();
    }

    public void BuildFolders()
    {
        Directory.CreateDirectory(Path_SavePath);
    }

}
