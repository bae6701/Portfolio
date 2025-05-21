using Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CombineBar : UI_Base
{
    enum GameObjects
    { 
        Horizontal,
        DeActive,
    }

    enum Images
    { 
        RarityCircle,
        MainCharacter_Icon,
    }
    enum Texts
    {
        Name,
        Description
    }

    enum Buttons
    {
        LeftArrow,
        RightArrow,
        CombineButton,
        CloseButton,
    }

    int pageindex = 0;
    Image MainCharacter_Icon;
    Transform Horizontal;
    List<GameObject> Gorvage = new List<GameObject>();
    List<HeroHolder> combineList = new List<HeroHolder>();

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        BindEvent(Get<Button>((int)Buttons.CloseButton).gameObject, OnClickCloseButton);
        BindEvent(Get<Button>((int)Buttons.LeftArrow).gameObject, OnClickLeftArrowButton);
        BindEvent(Get<Button>((int)Buttons.RightArrow).gameObject, OnClickRightArrowButton);
        BindEvent(Get<Button>((int)Buttons.CombineButton).gameObject, OnClickCombineButton);

        MainCharacter_Icon = Get<Image>((int)Images.MainCharacter_Icon);
        Horizontal = Get<GameObject>((int)GameObjects.Horizontal).transform;

        RefreshUI();
    }

    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(false);
    }

    public void OnClickCloseButton(PointerEventData evt)
    {
        CloseCombineBar();
        gameObject.GetComponent<Animator>().Play("CombineBar_Close");
    }

    public void OnClickCombineButton(PointerEventData evt)
    {
        Rarity SpawnHerorarity = Managers.Data.heroDict[Managers.Data.combineDict[pageindex].SpawnheroType].rarity;
        HeroType type = Managers.Data.combineDict[pageindex].SpawnheroType;
        foreach (var holder in combineList)
        {
            holder.Sell(true);
        }
        Managers.Spawn.Spawner.Summon(SpawnHerorarity, type);
        CloseCombineBar();
    }

    public void OnClickLeftArrowButton(PointerEventData evt)
    {
        Arrow(-1);
    }

    public void OnClickRightArrowButton(PointerEventData evt)
    {
        Arrow(1);
    }

    private void CloseCombineBar()
    {
        gameObject.GetComponent<Animator>().Play("CombineBar_Close");
    }

    public void RefreshUI()
    {
        if (Gorvage.Count > 0)
        {
            for (int i = 0; i < Gorvage.Count; i++) Destroy(Gorvage[i]);
            Gorvage.Clear();
        }

        combineList.Clear();
        var combineData = Managers.Data.combineDict[pageindex];
        MainCharacter_Icon.sprite = Managers.Data.GetAtlas(Enum.GetName(typeof(HeroType), (int)combineData.SpawnheroType));

        Get<TextMeshProUGUI>((int)Texts.Name).text = Managers.Data.heroDict[combineData.SpawnheroType].name;
        Get<TextMeshProUGUI>((int)Texts.Description).text = Managers.Data.heroDict[combineData.SpawnheroType].description;

        for (int i = 0; i < combineData.SpawnMaterialsType.Count; i++)
        {
            var go = Managers.Resource.Instantiate("UI/Popup/SubCharacter", Horizontal);

            go.transform.Find("SubCharacter_Icon").GetComponent<Image>().sprite =
                Managers.Data.GetAtlas(Enum.GetName(typeof(HeroType), (int)combineData.SpawnMaterialsType[i]));

            go.transform.Find("Circle").GetComponent<Image>().color =
                Net_Utils.RarityCircleColor(Managers.Data.heroDict[combineData.SpawnMaterialsType[i]].rarity);

            if (i != combineData.SpawnMaterialsType.Count - 1)
            {
                var plus = Managers.Resource.Instantiate("UI/Popup/Plus", Horizontal);
                Gorvage.Add(plus);
            }
            Gorvage.Add(go);
        }
        Get<GameObject>((int)GameObjects.DeActive).gameObject.SetActive(!CanCombine());
    }

    public bool CanCombine()
    {       
        var combineData = Managers.Data.combineDict[pageindex];
        var heroList = Managers.Spawn.SelectSpawnHeroList(Net_Utils.LocalId() == 0);
        
        foreach (var materialsType in combineData.SpawnMaterialsType)
        {
            for (int i = 0; i < heroList.Count; i++)
            {
                if (materialsType == heroList[i]._heroData.heroType)
                {
                    combineList.Add(heroList[i]._parentHolder);
                    break;
                }
            }
        }

        if (combineData.SpawnMaterialsType.Count == combineList.Count)
            return true;

        return false;
    }

    public void Arrow(int value)
    {
        pageindex += value;

        if (pageindex < 0)
        {
            pageindex = Managers.Data.combineDict.Count - 1;
        }
        else if (pageindex > Managers.Data.combineDict.Count - 1)
        { 
            pageindex =0;
        }
        RefreshUI();
    }
}
