using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_Battle : UI_Base
{

    enum Buttons
    {
        RandomMatch,
        WithFriends,
    }
    public string LobbyId;
    public UI_MatchingPopup MatchingPopup;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI WaveText;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        BindEvent(GetButton((int)Buttons.RandomMatch).gameObject, OnClickRandomMatching);
        SetUI();
    }

    public void SetUI()
    {
        LevelText.text = "Lv. " + Managers.Cloud.account.level.ToString();
        WaveText.text = Managers.Cloud.Wave.ToString();
        if (Managers.Cloud.Wave == 0)
        {
            WaveText.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    public void OnClickRandomMatching(PointerEventData evt)
    {
        MatchingPopup.gameObject.SetActive(true);
        Managers.Net.StartMatchmaking((res) =>
        {
            LobbyId = res;
            MatchingPopup.BindCancelButton(LobbyId);
        });
    }

    
}
