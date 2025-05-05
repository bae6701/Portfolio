using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_GameOver : UI_Base
{
    enum Texts
    {
        Wave_Text,
        Player_DPS,
        Other_DPS,
    }

    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI otherName;
    TextMeshProUGUI Player01_DPS;
    TextMeshProUGUI Player02_DPS;

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Player01_DPS = Get<TextMeshProUGUI>((int)Texts.Player_DPS);
        Player02_DPS = Get<TextMeshProUGUI>((int)Texts.Other_DPS);
    }
    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(false);
    }
    public void GameOver()
    {
        Get<TextMeshProUGUI>((int)Texts.Wave_Text).text = "WAVE " + Managers.Game.Wave.ToString();
        StartCoroutine(SceneLoadDelay());
    }

    IEnumerator SceneLoadDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Player01_DPS.text = string.Format("{0:0.0}", Managers.Game.Player_DPS);
        Player02_DPS.text = string.Format("{0:0.0}", Managers.Game.Other_DPS);
        playerName.text = Managers.Cloud.account.playerName;
        otherName.text = Managers.Cloud.other_account.playerName;
        yield return new WaitForSecondsRealtime(5.0f);
        Time.timeScale = 1.0f;
        if (NetworkManager.Singleton.IsHost)
        {
            Managers.Scene.LoadScene(Define.Scene.Lobby);
        }
    }    
}
