using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_UpGradeBar : UI_Base
{
    private TextMeshProUGUI Yellow_Asset_Text;
    private TextMeshProUGUI UpGradeLevel1_Text;
    private TextMeshProUGUI UpGradeLevel2_Text;
    private TextMeshProUGUI UpGradeLevel3_Text;
    private TextMeshProUGUI UpGradeLevel4_Text;
    private TextMeshProUGUI UpGradeSection1_Button_Text;
    private TextMeshProUGUI UpGradeSection2_Button_Text;
    private TextMeshProUGUI UpGradeSection3_Button_Text;

    enum Texts
    {
        UpGradeLevel1_Text,
        UpGradeLevel2_Text,
        UpGradeLevel3_Text,
        UpGradeLevel4_Text,

        UpGradeSection1_Button_Text,
        UpGradeSection2_Button_Text,
        UpGradeSection3_Button_Text,
        Summon_Button_Text,

        Yellow_Asset_Text,
        Blue_Asset_Text,
    }

    enum Buttons
    {
        UpGradeSection1_Button,
        UpGradeSection2_Button, 
        UpGradeSection3_Button,
        Summon_Button,
        CloseButton,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseUpgradeBar);

        GetButton((int)Buttons.UpGradeSection1_Button).gameObject.BindEvent(OnClickUpgradeButton);
        GetButton((int)Buttons.UpGradeSection2_Button).gameObject.BindEvent(OnClickUpgradeButton);
        GetButton((int)Buttons.UpGradeSection3_Button).gameObject.BindEvent(OnClickUpgradeButton);
        GetButton((int)Buttons.Summon_Button).gameObject.BindEvent(OnClickUpgradeButton);
        Yellow_Asset_Text = GetText((int)Texts.Yellow_Asset_Text);

        for (int i = 0; i < (int)Buttons.CloseButton; i++)
        {
            UpdateText(i);
        }
        

        UpGradeSection1_Button_Text = GetText((int)Texts.UpGradeSection1_Button_Text);
        UpGradeSection2_Button_Text = GetText((int)Texts.UpGradeSection2_Button_Text);
        UpGradeSection3_Button_Text = GetText((int)Texts.UpGradeSection3_Button_Text);
    }

    private void Update()
    {
        Yellow_Asset_Text.text = Managers.Game.Money.ToString();
    }

    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(false);
    }

    public void OnClickCloseUpgradeBar(PointerEventData evt)
    {     
        gameObject.GetComponent<Animator>().SetTrigger("Close");        
    }

    public void OnClickUpgradeButton(PointerEventData evt)
    {      
        for (int i = 0; i < (int)Buttons.CloseButton; i++)
        {
            if (evt.pointerEnter.name.Contains(Enum.GetName(typeof(Buttons), i)))
            {
                UpgradeHero(i);
                return;
            }
        }
    }

    public void UpgradeHero(int value)
    {
        if (Managers.Game.Money < Managers.Game.UpGradeLevelMoney[value]) return;

        Managers.Game.Money -= Managers.Game.UpGradeLevelMoney[value];

        Managers.Game.UpGradeLevel[value]++;
        Managers.Game.UpGradeLevelMoney[value]++;

        UpdateText(value);
    }


    public void UpdateText(int value)
    {
        GetText(value).text = "Lv. " + Managers.Game.UpGradeLevel[value].ToString();
        GetText(value+4).text = Managers.Game.UpGradeLevelMoney[value].ToString();    
    }

}
