using Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameScene_Bottom : UI_Base
{
    enum Buttons
    {
        UpgradeButton,
        Card,
    }

    public UI_Assets AssetsUI { get; private set; }
    public UI_Summon SummonUI { get; private set; }
    public UI_NavigationBar NaviUI { get; private set; }
    public UI_UpGradeBar UpGradeUI { get; private set; }
    public UI_CombineBar CombineUI { get; private set; }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(OnClickUpgradeButton);
        GetButton((int)Buttons.Card).gameObject.BindEvent(OnClickCombineButton);

        UpGradeUI = Utils.FindChild<UI_UpGradeBar>(this.gameObject, "UpGrade_Bar");
        AssetsUI = Utils.FindChild<UI_Assets>(this.gameObject, "Assets");
        SummonUI = Utils.FindChild<UI_Summon>(this.gameObject, "Summon_Button");
        NaviUI = Utils.FindChild<UI_NavigationBar>(this.gameObject, "Navigation_Bar");
        CombineUI = Utils.FindChild<UI_CombineBar>(this.gameObject, "Combine_Bar");      
    }

    #region ButtonEvent

    public void OnClickUpgradeButton(PointerEventData evt)
    {
        UpGradeUI.gameObject.SetActive(true);
    }

    public void OnClickCombineButton(PointerEventData evt)
    {
        CombineUI.RefreshUI();
        CombineUI.gameObject.SetActive(true);
        CombineUI.GetComponent<Animator>().Play("CombineBar_Open");
    }
    #endregion

    #region AssetsUI
    [ClientRpc]
    public void NotifyGetMoneyClientRpc(int value)
    {
        Managers.Game.Money += value;
        AssetsUI.Money_Anim();
    }
    #endregion
    
    #region NaviUI
    [ClientRpc]
    public void ClientNavigationClientRpc(ulong clientId, string temp)
    {
        string Host = Net_Utils.CheckHost(clientId) ? "내가 " : "상대방이 ";
        NaviUI.GetNavigation(Host + temp);
    }
    #endregion

    #region SummonUI

    #endregion
}
