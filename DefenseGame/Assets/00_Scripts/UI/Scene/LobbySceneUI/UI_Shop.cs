using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : UI_Base
{
    enum BuyGemButtons
    {
        Price150,
        Price330,
        Price850,
        Price1500,
        Price3300,
        Price5500,
    }
    //enum Texts
    //{ 
    //    Price150_Text,
    //    Price330_Text,
    //    Price850_Text,
    //    Price1500_Text,
    //    Price3300_Text,
    //    Price5500_Text,
    //}
    [SerializeField]  private Transform BuyGem;
    [SerializeField] private Button HeroSpawnOne;
    [SerializeField] private Button HeroSpawnTen;
    [SerializeField] private TextMeshProUGUI HeroSpawnOne_Text;
    [SerializeField] private TextMeshProUGUI HeroSpawnTen_Text;
    UI_Materials materials;

    public override void Init()
    {

        Bind<Button>(typeof(BuyGemButtons), BuyGem.gameObject);
        materials = Utils.FindChild<UI_Materials>(transform.parent.gameObject, "Materials");

        HeroSpawnOne.onClick.AddListener(() => UseGem(50));
        HeroSpawnTen.onClick.AddListener(() => UseGem(450));
        GetButton((int)(BuyGemButtons.Price150)).onClick.AddListener(()=> GetGem("dia150"));
        GetButton((int)(BuyGemButtons.Price150)).transform.Find("Price150_Text").GetComponent<TextMeshProUGUI>().text
            = string.Format("{0} {1}", Managers.Inapp.GetProduct("dia150").metadata.localizedPrice, Managers.Inapp.GetProduct("dia150").metadata.isoCurrencyCode);
        GetButton((int)(BuyGemButtons.Price330)).onClick.AddListener(()=> GetGem("dia330"));
        GetButton((int)(BuyGemButtons.Price330)).transform.Find("Price330_Text").GetComponent<TextMeshProUGUI>().text
            = string.Format("{0} {1}", Managers.Inapp.GetProduct("dia330").metadata.localizedPrice, Managers.Inapp.GetProduct("dia330").metadata.isoCurrencyCode);
        GetButton((int)(BuyGemButtons.Price850)).onClick.AddListener(()=> GetGem("dia850"));
        GetButton((int)(BuyGemButtons.Price850)).transform.Find("Price850_Text").GetComponent<TextMeshProUGUI>().text
            = string.Format("{0} {1}", Managers.Inapp.GetProduct("dia850").metadata.localizedPrice, Managers.Inapp.GetProduct("dia850").metadata.isoCurrencyCode);
        //Get<Button>((int)(BuyGemButtons.Price1500)).onClick.AddListener(()=> GetGem(1500));
        //Get<Button>((int)(BuyGemButtons.Price3300)).onClick.AddListener(()=> GetGem(3300));
        //Get<Button>((int)(BuyGemButtons.Price5500)).onClick.AddListener(()=> GetGem(5500));
        HeroSpawnButtonTextUpdate();
    }


    public void UseGem(int value)
    {
        if (Managers.Cloud.Gem < value) return;
        Managers.Cloud.Gem -= value;

        materials.GetMaterialUpdate();

        UI_GachaPopup gacha = Managers.UI.ShowPopupUI<UI_GachaPopup>();
        
        gacha.ReSummonAction = () =>
        {
            UseGem(value);
        };

        gacha.Initialize(value);
        HeroSpawnButtonTextUpdate();
    }

    public void HeroSpawnButtonTextUpdate()
    {
        HeroSpawnOne_Text.color = Managers.Cloud.Gem < 50 ? Color.red : Color.green;
        HeroSpawnTen_Text.color = Managers.Cloud.Gem < 450 ? Color.red : Color.green;
    }

    public void GetGem(string temp)
    {
        Managers.Inapp.Purchase(temp);
        
    }
    public void UpdateMaterials(int value)
    {
        Managers.Cloud.Gem += value;
        materials.GetMaterialUpdate();
        HeroSpawnButtonTextUpdate();
    }
}
