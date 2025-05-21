using Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Panel
{
    public GameObject obj;
    public Button button;
    public Image buttonImage;
    public Animator animator;
    public bool isActive = false;
}

public class UI_LobbySceneButtons : UI_Base
{
    public List<GameObject> gameobjects = new List<GameObject>();
    List<Panel> panels= new List<Panel>();

    public override void Init()
    {
        Bind<Button>(typeof(LobbyButtons));

        for (int i = 0; i < Enum.GetValues(typeof(LobbyButtons)).Length; i++)
        {
            Panel panel = new Panel();
            panel.obj = gameobjects[i];
            panel.button = GetButton(i);
            panel.buttonImage = GetButton(i).GetComponent<Image>();
            panel.animator = GetButton(i).GetComponent<Animator>();

            panels.Add(panel);

            int index = i;
            panels[i].button.onClick.AddListener(() => OnPanel(index));
        }       
    }

    public void OnPanel(int index)
    {
        if (panels[index].isActive) return;

        for (int i = 0; i < panels.Count; i++)
        {           
            if (panels[i].isActive == true)
            {
                panels[i].isActive = false;
                panels[i].animator.Play("DOWN");
                panels[i].buttonImage.color = Color.white;
            }
            panels[i].obj.SetActive(false);          
        }
        panels[index].obj.SetActive(true);
        panels[index].isActive = true;
        panels[index].animator.Play("UP");
        panels[index].buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
    }
}
