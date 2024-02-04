using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class ConsoleCommands : MonoBehaviour
{

    public InputField inputField;
    public Text consoleText;

    public delegate bool OnExecuteCommand(string commandName, string[] args); //Return success
    public static event OnExecuteCommand onExecuteCommand;

    private List<string> historyCommands = new List<string>()
    {
        "",
    };

    private int index = 0;

    private static ConsoleCommands _instance;

    public static ConsoleCommands Instance
    {
        get
        {
            if (_instance == null)
                _instance = Shopkeeper.Console;

            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (inputField.isFocused == false)
        {
            inputField.Select();
            inputField.ActivateInputField();
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            index = 0;
            CommandInput(inputField.text);
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            index++;
            if (index >= historyCommands.Count)
            {
                index = historyCommands.Count - 1;
            }
            inputField.SetTextWithoutNotify(historyCommands[index]);
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            index--;
            if (index < 0)
            {
                index = 0;
            }
            inputField.SetTextWithoutNotify(historyCommands[index]);
        }
    }

    public void CommandInput(string command)
    {
        string[] inputSplit = command.Split(' ');

        string commandInput = inputSplit[0];
        string[] args = inputSplit.Skip(1).ToArray();

        SendConsoleMessage("<color=#ffffffcc>" + command + "</color>");

        ProcessCommand(commandInput, args);

        historyCommands.Insert(0, command);
        inputField.Select();
    }

    public void SendConsoleMessage(string msg)
    {
        if (consoleText != null) consoleText.text += "> " + msg + "\n";
    }

    public void ProcessCommand(string commandInput, string[] args)
    {
        var listCommands = GetAll().ToList();

        foreach (var command in listCommands)
        {
            if (command.CommandName == commandInput)
            {
                Debug.Log($"Command executed: {command.CommandName}");
                command.ExecuteCommand(args);
                break;
            }
        }
    }

    [FoldoutGroup("DEBUG")]
    [Button("Test_ConsoleCommand")]
    public void Test_AllConsoleCommands()
    {
        var listCommands = GetAll().ToList();

        foreach(var command in listCommands)
        {
            Debug.Log(command.CommandName);
        }
    }

    IEnumerable<CC_Base> GetAll()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(CC_Base)))
            .Select(type => Activator.CreateInstance(type) as CC_Base);
    }

}
