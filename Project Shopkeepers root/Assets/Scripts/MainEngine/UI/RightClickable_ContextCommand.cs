using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ContextCommandElement = ContextCommand.ContextCommandElement;


public class RightClickable_ContextCommand : MonoBehaviour, IPointerClickHandler
{

    private List<ContextCommandElement> commands = new List<ContextCommandElement>();

    public List<ContextCommandElement> Commands { get => commands; set => commands = value; } //Can be set at will

    private void Start()
    {
        //ContextCommandElement c1 = new ContextCommandElement(Test123, "Only Test");
        //ContextCommandElement c2 = new ContextCommandElement(Test123, "Add Item");
        //c1.param = new string[1] { "waduh" };
        //c2.param = new string[1] { "struck" };
        //commands.Add(c1);
        //commands.Add(c2);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ContextCommand.ShowTooltip(commands);
        }
    }


    private void Test123(string[] allparams)
    {
        //if (allparams.Length > 0)
        //{
        //    if (allparams[0] == "waduh")
        //    {
        //        Debug.Log("wadi! daw!");

        //    }
        //    else if (allparams[0] == "struck")
        //    {
        //        Debug.Log("heartstrucken");
        //    }
        //}
    }
 
}
