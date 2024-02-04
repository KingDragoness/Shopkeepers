using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// UI Database contains all of Shopkeeper's UI elements.
/// </summary>


public class UIDatabase : MonoBehaviour
{

    public SKUI_FilepathField prefab_FileLocationField;
    public SKUI_InputField prefab_TextInputField;
    public ModularWindow prefab_ModularWindow;
    public GameObject assetCreatorUI;

    private void Start()
    {
        Shopkeeper.Console.gameObject.SetActive(false);
        assetCreatorUI.gameObject.SetActive(false);
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

}
