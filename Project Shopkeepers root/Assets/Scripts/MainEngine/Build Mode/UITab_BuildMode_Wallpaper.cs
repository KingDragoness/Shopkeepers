using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using ToolBox.Pools;

public class UITab_BuildMode_Wallpaper : MonoBehaviour
{

    public UITab_BuildMode_WallpaperButton prefab;
    public Transform parentButton;

    private List<UITab_BuildMode_WallpaperButton> allButtons = new List<UITab_BuildMode_WallpaperButton>();
    private UITab_BuildMode_WallpaperButton currentButton;

    private void Start()
    {
        prefab.gameObject.EnableGameobject(false);
    }

    private void OnEnable()
    {
        currentButton = null;
        RefreshUI();
    }

    private void Awake()
    {
        prefab.gameObject.Populate(10);
    }



    public void SelectWallpaper(UITab_BuildMode_WallpaperButton button)
    {
        BuildMode.Wallpaper.SetWallpaper(button.skTextureRef);
        currentButton = button;

        RefreshUI();
    }

    private void RefreshUI()
    {
        foreach(var button in allButtons)
        {
            button.button.onClick.RemoveAllListeners();
        }

        allButtons.ReleasePoolObject();
        int selectedIndex = -1;
        if (currentButton != null) selectedIndex = currentButton.index;
        int index = 0;

        foreach (var meta in BuildMode_Wallpaper.GetAllWallpaperTextures())
        {
            var texture = Shopkeeper.Database.Load_SKTexture(meta);
            var button = prefab.gameObject.Reuse<UITab_BuildMode_WallpaperButton>(parentButton); //Instantiate(prefab, parentButton);
            button.gameObject.SetActive(true);
            button.button.onClick.AddListener(() => SelectWallpaper(button));
            button.skTextureRef = meta;
            button.index = index;

            if (currentButton != null)
            {
                if (selectedIndex == button.index)
                {
                    button.button.interactable = false;
                }
                else
                {
                    button.button.interactable = true;

                }

                //if (currentButton.skTextureRef == button.skTextureRef)
                //{
                //    button.button.interactable = false;
                //}
                //else
                //{
                //    button.button.interactable = true;
                //}
            }
            else
            {
                button.button.interactable = true;
            }

            button.image.texture = texture;
            index++;
            allButtons.Add(button);

        }
    }


}
