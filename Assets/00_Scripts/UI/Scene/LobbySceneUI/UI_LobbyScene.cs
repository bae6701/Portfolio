using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Data;

public class UI_LobbyScene : UI_Scene
{
    enum GameObjects
    {
        Hero,
        Guild,
        Battle,
        Shop,
    }

    public UI_Hero HeroUI { get; private set; }
    public UI_Battle BattleUI { get; private set; }
    public UI_Shop ShopUI { get; private set; }
    //public SetNickNamePopup NickUI { get; private set; }
    public UI_LobbySceneButtons ButtonsUI { get; private set; }
    public UI_Materials MaterialsUI { get; private set; }

    public int pageIndex = (int)GameObjects.Battle;

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(LobbyButtons));
        BattleUI = GetObject((int)GameObjects.Battle).GetComponent<UI_Battle>();
        HeroUI = GetObject((int)GameObjects.Hero).GetComponent<UI_Hero>();
        ShopUI = GetObject((int)GameObjects.Shop).GetComponent<UI_Shop>();
        ButtonsUI = Utils.FindChild<UI_LobbySceneButtons>(this.gameObject, "Buttons");
        MaterialsUI = Utils.FindChild<UI_Materials>(this.gameObject, "Materials");
        //NickUI = Utils.FindChild<SetNickNamePopup>(this.gameObject, "SetNickName");
       
        MaterialsUI.GetMaterialUpdate();
        ButtonsUI.OnPanel((int)LobbyButtons.Battle);
        
    }
}
