using TMPro;
using UnityEngine;

public class UI_Materials : UI_Base
{
    enum Texts
    { 
        Energy_Text,
        Gold_Text,
        Gem_Text,
    }
    
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void GetMaterialUpdate()
    {
        GetText((int)Texts.Energy_Text).text = Managers.Cloud.Energy.ToString() + "/30";
        GetText((int)Texts.Gold_Text).text = Managers.Cloud.Gold.ToString();
        GetText((int)Texts.Gem_Text).text = Managers.Cloud.Gem.ToString();
    }
}
