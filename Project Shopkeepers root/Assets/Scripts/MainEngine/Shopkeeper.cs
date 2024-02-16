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

    [SerializeField] private ShopkeeperGame _game;
    [SerializeField] private DatabaseAssetHandler _database;
    [SerializeField] private AssetCreator _assetCreator;
    [SerializeField] private MainUI _UIDatabase;
    [SerializeField] private ConsoleCommands _consoleCommand;

    public static ShopkeeperGame Game
    {
        get
        {
            return Instance._game;
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
