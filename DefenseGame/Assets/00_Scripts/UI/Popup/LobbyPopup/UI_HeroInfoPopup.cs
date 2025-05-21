using Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_HeroInfoPopup : UI_Popup
{
    enum Buttons
    { 
        UpGradeButton,
        CloseButton,
    }

    [SerializeField] private TextMeshProUGUI HeroLevel_Text;
    [SerializeField] private TextMeshProUGUI Level1_Text;
    [SerializeField] private TextMeshProUGUI Passive1_Text;
    [SerializeField] private TextMeshProUGUI Level2_Text;
    [SerializeField] private TextMeshProUGUI Passive2_Text;
    [SerializeField] private TextMeshProUGUI Level3_Text;
    [SerializeField] private TextMeshProUGUI Passive3_Text;
    [SerializeField] private TextMeshProUGUI Gold_Text;
    [SerializeField] private TextMeshProUGUI HeroName_Text;
    [SerializeField] private GameObject Level1_Lock;
    [SerializeField] private GameObject Level2_Lock;
    [SerializeField] private GameObject Level3_Lock;
    [SerializeField] private Image Rarity;
    [SerializeField] private Image HeroImage;

    HeroData heroData = null;

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        BindEvent(GetButton((int)Buttons.CloseButton).gameObject, OnClickCloseButton);
        BindEvent(GetButton((int)Buttons.UpGradeButton).gameObject, OnClickUpGradeButton);
    }

    public void OnClickCloseButton(PointerEventData evt)
    {
        ClosePopupUI();
    }

    public void OnClickUpGradeButton(PointerEventData evt)
    {
        var heroBox = Managers.Cloud.account.heroData;
        if(Managers.Cloud.account.gold < UpgradeCosts(heroBox[heroData.heroType].level)) return;

        Managers.Cloud.account.gold -= UpgradeCosts(heroBox[heroData.heroType].level);
        heroBox[heroData.heroType].level++;
        UpGradeUpdate();
    }

    public void SetInfoData(HeroData heroData)
    {
        this.heroData = heroData;
        var dict = Managers.Data.heroPassiveDict;
        var heroBox = Managers.Cloud.account.heroData;
        Rarity.color = Net_Utils.RarityCircleColor(heroData.rarity);
        HeroImage.sprite = Managers.Data.GetAtlas(Enum.GetName(typeof(HeroType), (int)heroData.heroType));
        HeroImage.gameObject.GetOrAddComponent<Animator>().runtimeAnimatorController = heroData.animator;
        HeroLevel_Text.text = "Lv. " + heroBox[heroData.heroType].level.ToString();
        Level1_Text.text = "Lv. " + dict[heroData.heroType].passives[0].openLevel.ToString();
        Passive1_Text.text = dict[heroData.heroType].passives[0].description.ToString();
        Level2_Text.text = "Lv. " + dict[heroData.heroType].passives[1].openLevel.ToString();
        Passive2_Text.text = dict[heroData.heroType].passives[1].description.ToString();
        Level3_Text.text = "Lv. " + dict[heroData.heroType].passives[2].openLevel.ToString();
        Passive3_Text.text = dict[heroData.heroType].passives[2].description.ToString();
        Gold_Text.text = UpgradeCosts(heroBox[heroData.heroType].level).ToString();
        Gold_Text.color = Managers.Cloud.account.gold >= UpgradeCosts(heroBox[heroData.heroType].level) ? Color.white : Color.red;
        HeroName_Text.text = heroData.name;
        UpGradeUpdate();
    }

    public void UpGradeUpdate()
    {
        var heroBox = Managers.Cloud.account.heroData;
        var dict = Managers.Data.heroPassiveDict;
        if (heroBox[heroData.heroType].level >= dict[heroData.heroType].passives[0].openLevel)
        {
            Level1_Lock.SetActive(false);
            if (heroBox[heroData.heroType].level >= dict[heroData.heroType].passives[1].openLevel)
            {
                Level2_Lock.SetActive(false);
                if (heroBox[heroData.heroType].level >= dict[heroData.heroType].passives[2].openLevel)
                {
                    Level3_Lock.SetActive(false);
                }
                else return;
            }
            else return;
        }
        else return;
    }

    public int UpgradeCosts(int level)
    {
        return Mathf.RoundToInt(500 * Mathf.Pow(level, 1.2f));
    }
}
