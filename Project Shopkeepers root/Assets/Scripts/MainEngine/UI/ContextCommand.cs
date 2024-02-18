using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;


public class ContextCommand : MonoBehaviour
{

    [System.Serializable]
    public class ContextCommandElement
    {
        public contextCommand delegateCommand;
        public UnityEvent OnActionCommand;
        public string[] param;
        public string buttonText;

        public ContextCommandElement(contextCommand delegateCommand, string _buttonText, UnityEvent onActionCommand = null)
        {
            this.delegateCommand = delegateCommand;
            this.OnActionCommand = onActionCommand;
            this.buttonText = _buttonText;
        }
    }

    public SKUI_Button prefab_ContextTooltipButton;
    public List<SKUI_Button> allContextButtons = new List<SKUI_Button>();
    public RectTransform rectTransform;
    public Vector2 offset = new Vector2(2f, 6f);
    [SerializeField] [ReadOnly] private List<ContextCommandElement> allContextCommandsCached = new List<ContextCommandElement>();

    public delegate void contextCommand(string[] param);

    public delegate void onContextCommandClosed();
    public static event onContextCommandClosed OnContextCommandClosed;

    public static ContextCommand instance
    {
        get { return Shopkeeper.UI.ContextCommand; }
    }

    public static void ShowTooltip(List<ContextCommandElement> commands)
    {
        instance.ShowTooltip_1(commands);
    }

    public void ShowTooltip_1(List<ContextCommandElement> commands)
    {
        SetPosition();

        foreach (var button in instance.allContextButtons)
        {
            if (button == null) continue;
            Destroy(button.gameObject);
        }

        allContextButtons.Clear();

        allContextCommandsCached = commands;
        gameObject.SetActive(true);
        int index = 0;
        //create commands
        foreach (var command in commands)
        {
            var newButton = Instantiate(prefab_ContextTooltipButton, rectTransform.transform);
            newButton.transform.localScale = Vector3.one;
            newButton.label.text = command.buttonText;
            newButton.index = index; 
            int z = index; 
            newButton.button.onClick.AddListener(() => ExecuteCommand(z)); //TIDAKKKKKKKKKKKKKKK INT PASS BY REFERENCE HARUS BIKIN int z to prevent this
            newButton.gameObject.EnableGameobject(true);
            allContextButtons.Add(newButton);
            index++;
        }

        //RefreshUI();
    }


    private void SetPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 posTarget = new Vector2(mousePos.x, mousePos.y) + offset;
        posTarget.x = Mathf.Floor(posTarget.x);
        posTarget.y = Mathf.Floor(posTarget.y - rectTransform.sizeDelta.y);

        if (posTarget.x < 0f) posTarget.x = 0f;
        if (posTarget.y < 0f) posTarget.y = 0f;
        if (posTarget.x > Screen.width - rectTransform.sizeDelta.x) posTarget.x = Screen.width - rectTransform.sizeDelta.x;
        if (posTarget.y > Screen.height - rectTransform.sizeDelta.y) posTarget.y = Screen.height - rectTransform.sizeDelta.y;

        rectTransform.anchoredPosition = posTarget;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) | Input.GetMouseButtonDown(1))
        {
            HandleCloseContextCommand();
        }
    }

    private void HandleCloseContextCommand()
    {
        var rr = MainUI.GetEventSystemRaycastResults();

        foreach (var result in rr)
        {
            if (result.gameObject.IsParentOf(gameObject))
            {
                return;
            }
            else
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }

    public void ExecuteCommand(int index)
    {
        var commandElement = allContextCommandsCached[index];

        commandElement.OnActionCommand?.Invoke();
        commandElement.delegateCommand?.Invoke(commandElement.param);
    }


    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }



}
