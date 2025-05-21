using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MatchingPopup : UI_Popup
{
    enum Buttons
    {
        CancelButton,
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
    }
    string LobbyId;
    public void OnClickCancelButton(PointerEventData evt)
    {
        gameObject.SetActive(false);
        Managers.Net.DestroyLobby(LobbyId);
    }

    public void BindCancelButton(string lobbyId)
    {
        LobbyId = lobbyId;
        BindEvent(GetButton((int)Buttons.CancelButton).gameObject, OnClickCancelButton);
    }
}
