using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_NickSetPopup : UI_Popup
{
    TMP_InputField NickNameField;
    TextMeshProUGUI NotiText;
    Button Okbutton;

    public Action ShowLobbySceneAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Init()
    {
        base.Init();
        NickNameField = Utils.FindChild<TMP_InputField>(this.gameObject, "Input", true);
        NotiText = Utils.FindChild<TextMeshProUGUI>(this.gameObject, "NotiText", true);
        Okbutton = Utils.FindChild<Button>(this.gameObject, "OkButton", true);

        BindEvent(Okbutton.gameObject, OnClickOkButton);
    }

    public void OnClickOkButton(PointerEventData evt)
    {
        SetNickName();
    }

    public void SetNickName()
    {
        string nickname = NickNameField.text;
        if (string.IsNullOrEmpty(nickname))
        {
            NotiText.text = "닉네임을 입력해주세요.";
            return;
        }
        if (nickname.Length < 3 || nickname.Length > 10)
        {
            NotiText.text = "닉네임은 3글자 이상 10글자 이하로 만들어주셔야합니다.";
            return;
        }

        Managers.Cloud.account.playerName = NickNameField.text;
        Managers.Cloud.Save();

        ClosePopupUI();

        ShowLobbySceneAction?.Invoke();
    }
}
