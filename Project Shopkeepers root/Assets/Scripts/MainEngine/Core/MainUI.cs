using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using Sirenix.OdinInspector;

/// <summary>
/// UI Database contains all of Shopkeeper's UI elements.
/// </summary>


public class MainUI : MonoBehaviour
{

    [SerializeField] private WindowConfirmPrompt window_ConfirmPrompt;
    public ModularWindow prefab_ModularWindow;
    [SerializeField] private Tooltip _tooltipHighlight;
    [SerializeField] private ContextCommand _contextTooltip;
    public Vector2 inGame_borderMin = new Vector2(0, 62); //from left,bottom
    public Vector2 inGame_borderMax = new Vector2(0, 26); //to right,up

    [Space]
    [FoldoutGroup("Ui Elements")] [SerializeField] private SKUI_FilepathField prefab_FileLocationField;
    [FoldoutGroup("Ui Elements")] [SerializeField] private SKUI_InputField prefab_TextInputField;
    [FoldoutGroup("Ui Elements")] [SerializeField] private SKUI_Vector3IntField prefab_Vector3InputField;
    [FoldoutGroup("Ui Elements")] [SerializeField] private SKUI_Bool prefab_BoolField;
    [FoldoutGroup("Ui Elements")] [SerializeField] private SKUI_Dropdown prefab_DropdownField;
    [FoldoutGroup("Ui Elements")] [SerializeField] private SKUI_Button prefab_ButtonField;
    [FoldoutGroup("Canvas")] [SerializeField] private Canvas GameCanvas;
    [FoldoutGroup("Canvas")] [SerializeField] private Canvas AssetCreatorCanvas;
    [FoldoutGroup("Canvas")] [SerializeField] private Canvas ModalPromptCanvas;
    public GameObject assetCreatorUI;


    public RectTransform rt_GameCanvas 
    { 
        get { return GameCanvas.GetComponent<RectTransform>(); } 
    }

    public WindowConfirmPrompt ConfirmPrompt { get => window_ConfirmPrompt; }
    public Tooltip Tooltip { get => _tooltipHighlight; }
    public ContextCommand ContextCommand { get => _contextTooltip; }


    private void Start()
    {
        Shopkeeper.Console.gameObject.SetActive(false);
        assetCreatorUI.gameObject.SetActive(false);
        ContextCommand.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.BackQuote))
        {
            Shopkeeper.Console.gameObject.SetActive(!Shopkeeper.Console.gameObject.activeSelf);
        }
    }

    public SKUI_FilepathField DrawFileLocationField(Transform parent, string label = "asset loc:", string defaultText = "", string placeholderText = "Enter text...")
    {
        var _inputfield = Instantiate(prefab_FileLocationField);
        var _placeholderText = _inputfield.inputField.placeholder.GetComponent<Text>();

        _inputfield.label.text = label;
        _placeholderText.text = placeholderText;
        _inputfield.inputField.SetTextWithoutNotify(defaultText);
        _inputfield.gameObject.SetActive(true);
        _inputfield.transform.SetParent(parent);
        _inputfield.transform.localScale = Vector3.one;
        return _inputfield;
    }

    public SKUI_Bool DrawBoolField(Transform parent, string label = "Is Active")
    {
        var _boolField = Instantiate(prefab_BoolField);
        _boolField.label.text = label;
        _boolField.gameObject.SetActive(true);
        _boolField.transform.SetParent(parent);
        _boolField.transform.localScale = Vector3.one;
        return _boolField;
    }

    public SKUI_Button DrawButtonField(Transform parent, string label = "Open tool", float size = 28f)
    {
        var _buttonField = Instantiate(prefab_ButtonField);
        var _rtButton = _buttonField.GetComponent<RectTransform>();
        Vector2 sizeDelta = _rtButton.sizeDelta;
        sizeDelta.y = size;

        _buttonField.label.text = label;
        _buttonField.gameObject.SetActive(true);
        _buttonField.transform.SetParent(parent);
        _buttonField.transform.localScale = Vector3.one;
        _rtButton.sizeDelta = sizeDelta;
        return _buttonField;
    }


    public SKUI_Dropdown DrawDropdown(Transform parent, string[] options, string label = "Resolution")
    {
        var _dropdownField = Instantiate(prefab_DropdownField);
        _dropdownField.label.text = label;

        List<Dropdown.OptionData> allOptionDatas = new List<Dropdown.OptionData>();
        for(int x = 0; x < options.Length; x++)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = options[x];

            allOptionDatas.Add(od);
        }


        _dropdownField.dropdown.options = allOptionDatas;
        _dropdownField.gameObject.SetActive(true);
        _dropdownField.transform.SetParent(parent);
        _dropdownField.transform.localScale = Vector3.one; 
        return _dropdownField;
    }

    public SKUI_InputField DrawTextInputField(Transform parent, string label = "asset loc:", string defaultText = "", string placeholderText = "Enter text...")
    {
        var _inputfield = Instantiate(prefab_TextInputField);
        var _placeholderText = _inputfield.inputfield.placeholder.GetComponent<Text>();

        _inputfield.label.text = label;
        _placeholderText.text = placeholderText;
        _inputfield.inputfield.SetTextWithoutNotify(defaultText);
        _inputfield.gameObject.SetActive(true);
        _inputfield.transform.SetParent(parent);
        _inputfield.transform.localScale = Vector3.one;
        return _inputfield;
    }

    public SKUI_Vector3IntField DrawVector3IntField(Transform parent, string label_x = "x", string label_y = "y", string label_z = "z")
    {
        var _inputfield = Instantiate(prefab_Vector3InputField);

        _inputfield.label_x.text = label_x;
        _inputfield.label_y.text = label_y;
        _inputfield.label_z.text = label_z;
        _inputfield.gameObject.SetActive(true);
        _inputfield.transform.SetParent(parent);
        _inputfield.transform.localScale = Vector3.one;
        return _inputfield;
    }
    public static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
