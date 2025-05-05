using Data;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GachaPopup : UI_Popup
{
    enum Buttons
    { 
        ClosePopup,
        ReSummon,
    }

    enum Transforms
    { 
        Grid,        
    }

    public Action ReSummonAction = null;
    [SerializeField] private TextMeshProUGUI SpawnButton_Text;    
    public override void Init()
    {
        base.Init();

        Bind<Transform>(typeof(Transforms));
        Bind<Button>(typeof(Buttons));

        BindEvent(GetButton((int)Buttons.ClosePopup).gameObject, OnClickCloseButton);     
    }
    public void Initialize(int value)
    {
        int count = value == 50 ? 1 : 10;

        SpawnButton_Text.text = value.ToString();
        SpawnButton_Text.color = Managers.Cloud.Gem < value ? Color.red : Color.green;

        BindEvent(GetButton((int)Buttons.ReSummon).gameObject, OnClickReSummonButton);

        StartCoroutine(Gacha_Coroutine(count));
    }
    IEnumerator Gacha_Coroutine(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float roll = UnityEngine.Random.Range(0.0f, 100.0f);
            Rarity rarity = DetermineRarity(roll);
            HeroType type = Net_Utils.RandomsSpawn(rarity);
            int RandomCount = UnityEngine.Random.Range(1, 11);

            GameObject go = Managers.Resource.Instantiate("Character/GachaHero", Get<Transform>((int)Transforms.Grid));
            go.transform.Find("Rarity").GetComponent<Image>().color = Net_Utils.RarityCircleColor(rarity);
            go.transform.Find("Hero").GetComponent<Image>().sprite = Managers.Data.GetAtlas(Enum.GetName(typeof(HeroType), (int)type));
            go.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = RandomCount.ToString();
            GetHero(type, RandomCount);           
            yield return new WaitForSeconds(0.1f);
        }

    }

    public void OnClickCloseButton(PointerEventData evt)
    {
        ClosePopupUI();
    }
    public void OnClickReSummonButton(PointerEventData evt)
    {
        ClosePopupUI();
        ReSummonAction?.Invoke();
    }

    private Rarity DetermineRarity(float roll)
    {
        var dict = Managers.Data.settingDict;
        float totalweight = 0f;
        foreach (var value in dict)
        {
            totalweight += value.Value;
            if (roll <= totalweight)
            {
                return value.Key;
            }
        }

        return 0;
    }

    public void GetHero(HeroType type, int count)
    {
        Managers.Cloud.account.heroData[type].count += count;
        Managers.Cloud.Save();
        UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
        lobbyScene.HeroUI.SetHeroDict();
    }
}
