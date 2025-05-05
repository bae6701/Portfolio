using Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_HeroListBlock : UI_Base
{
    enum Images
    { 
        Rarity,
        Hero,
    }

    enum Buttons
    { 
        Count,
    }

    enum Texts
    { 
        Count_Text,
        HeroName
    }

    enum GameObjects
    { 
        UpGrade,
    }

    [SerializeField] private Image Fill;

    private HeroData data = null;
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        BindEvent(GetButton((int)Buttons.Count).gameObject, OnClickInfoButton);
        
    }

    public void SetHeroBlock(HeroData heroData)
    {
        data = heroData;
        var heroBox = Managers.Cloud.account.heroData;
        int upgradeCount = UpgradeCosts(heroBox[heroData.heroType].level);
        int getCount = heroBox[heroData.heroType].count;
        GetImage((int)Images.Rarity).color = Net_Utils.RarityCircleColor(heroData.rarity);
        GetImage((int)Images.Hero).sprite = Managers.Data.GetAtlas(Enum.GetName(typeof(HeroType), (int)heroData.heroType));
        GetText((int)Texts.Count_Text).text = getCount.ToString() + " / " + upgradeCount.ToString();
        GetObject((int)GameObjects.UpGrade).SetActive(getCount >= upgradeCount);
        GetText((int)Texts.HeroName).text = heroData.name;
        Fill.fillAmount = (float)getCount / (float)upgradeCount;
    }

    public void OnClickInfoButton(PointerEventData evt)
    {
        if (data != null)
        {
            HeroInfoPopup();
        }
    }

    public void HeroInfoPopup()
    {
        UI_HeroInfoPopup heroInfo = Managers.UI.ShowPopupUI<UI_HeroInfoPopup>();
        heroInfo.SetInfoData(data);
    }

    public int UpgradeCosts(int level)
    {
        return Mathf.RoundToInt(10 * Mathf.Pow(level, 1.2f));
    }
}
