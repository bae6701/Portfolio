using System;
using UnityEngine;

public class LobbyScene : BaseScene
{
    UI_LobbyScene _sceneUI;

    protected override async void Init()
    {
        base.Init();

        SceneType = Define.Scene.Lobby;

        UI_Loading loadingpopup = Managers.UI.ShowPopupUI<UI_Loading>();

        await Managers.Net.Init(loadingpopup);

        Managers.UI.ClosePopupUI(loadingpopup);

        if (string.IsNullOrEmpty(Managers.Cloud.account.playerName))
        {
            UI_NickSetPopup nickNamepopup = Managers.UI.ShowPopupUI<UI_NickSetPopup>();
            nickNamepopup.ShowLobbySceneAction = () =>
            {
                _sceneUI = Managers.UI.ShowSceneUI<UI_LobbyScene>();
            };
        }
        else
        { 
            _sceneUI = Managers.UI.ShowSceneUI<UI_LobbyScene>();        
        }
    }

    public override void Clear()
    {
    
    }
}
