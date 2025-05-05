using Data;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_Assets : UI_Base
{
    enum Texts
    { 
        Yellow_Asset_Text,
        Blue_Asset_Text,
        Hero_Count_Text,
    }

    [SerializeField] private Animator MoneyAnimator;
    private TextMeshProUGUI Money_T;
    private TextMeshProUGUI HeroCount_T;
    List<Hero> list;

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        Money_T = GetText((int)Texts.Yellow_Asset_Text);
        HeroCount_T = GetText((int)Texts.Hero_Count_Text);
        list = Managers.Spawn.SelectSpawnHeroList(Net_Utils.LocalId() == 0);
    }
    private void Update()
    { 
        HeroCount_T.text = list.Count.ToString("00") + " / " + Managers.Game.HeroMaxCount.ToString("00");
        Money_T.text = Managers.Game.Money.ToString();
    }
    
    public void Money_Anim()
    {
        MoneyAnimator.SetTrigger("GET");
    }  
}
