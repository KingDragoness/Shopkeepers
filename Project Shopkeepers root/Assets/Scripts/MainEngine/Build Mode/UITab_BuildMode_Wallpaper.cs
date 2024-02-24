using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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



    public void SelectWallpaper(UITab_BuildMode_WallpaperButton button)
    {
        BuildMode.Wallpaper.SetWallpaper(button.skTextureRef);
        currentButton = button;

        RefreshUI();
    }

    private void RefreshUI()
    {
        allButtons.DestroyAndClearList();

        foreach(var meta in BuildMode_Wallpaper.GetAllWallpaperTextures())
        {
            var texture = Shopkeeper.Database.Load_SKTexture(meta);
            var button = Instantiate(prefab, parentButton);
            button.gameObject.SetActive(true);
            button.button.onClick.AddListener(() => SelectWallpaper(button));
            button.skTextureRef = meta;

            if (currentButton != null)
            {
                if (currentButton.skTextureRef == button.skTextureRef)
                {
                    button.button.interactable = false;
                }
            }

            button.image.texture = texture;
            allButtons.Add(button);

        }
    }


}
