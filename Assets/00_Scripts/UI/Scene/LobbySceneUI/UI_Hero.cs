using Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hero : UI_Base
{
    [SerializeField] private Transform listparent;

    Dictionary<HeroData, UI_HeroListBlock> herodict = new Dictionary<HeroData, UI_HeroListBlock>();
    public override void Init()
    {
        foreach (var hero in Managers.Data.heroDict)
        {            
            GameObject go = Managers.Resource.Instantiate("Character/HeroListBlock", listparent);
            UI_HeroListBlock heroBlock = go.GetOrAddComponent<UI_HeroListBlock>();
            herodict.Add(hero.Value, heroBlock);
            heroBlock.SetHeroBlock(hero.Value);
        }
    }
    public void SetHeroDict()
    {
        foreach (var herodata in herodict)
        {
            herodata.Value.SetHeroBlock(herodata.Key);
        }
    }
}
