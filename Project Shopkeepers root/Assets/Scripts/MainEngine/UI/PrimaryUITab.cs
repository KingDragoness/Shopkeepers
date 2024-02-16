using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using SFB;
using ButtonCommandElement = WindowConfirmPrompt.ButtonCommandElement;

/// <summary>
/// Primary UI is the 4 main tabs (Perks, Management, Inventory, Finance).
/// </summary>
public class PrimaryUITab : MonoBehaviour
{

    public Transform parentContent;
    public RectTransform rectTransform;

    [FoldoutGroup("Header")] public Text label_header;
    [FoldoutGroup("Header")] public Image image_icon;

    private bool _initiateDragWindow = false;
    private bool _initiateResizing = false;
    private Vector2 _deltaDragPos;
    private float _cooldownDrag = 0.2f;
    private float _cooldownResize = 0.2f;

    public virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }



}
