using Data;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_NavigationBar : UI_Base
{
    List<GameObject> NavigationTextList = new List<GameObject>();

    public override void Init()
    {
        
    }

    public void GetNavigation(string temp)
    {
        if (NavigationTextList.Count > 7)
        {
            NavigationTextList.RemoveAt(0);
            Managers.Resource.Destroy(NavigationTextList[0]);
        }
        var go = Managers.Resource.Instantiate("UI/Scene/Navigation_Text", this.transform);
        NavigationTextList.Add(go);
        go.transform.SetAsFirstSibling();
        go.GetComponent<TextMeshProUGUI>().text = temp;
        Managers.Resource.Destroy(go, 2.5f);
    } 
}
