using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GameScene_Middle : UI_Base
{
    public UI_GameOver GamaOverUI { get; private set; }
  
    public override void Init()
    {
        GamaOverUI = GetComponentInChildren<UI_GameOver>();             
    }

    [ClientRpc]
    private void SendDpsToClientRpc(double hostDps, double clientDps)
    {
        if (!IsServer)
        {
            Debug.Log($"Player_DPS : {clientDps} / Other_DPS : {hostDps}");
            Managers.Game.Player_DPS = clientDps;
            Managers.Game.Other_DPS = hostDps;          
        }
        GamaOverUI.GameOver();       
    }
    [ClientRpc]
    public void GameOverClientRpc()
    {
        GamaOverUI.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
        if (IsServer)
        {
            SendDpsToClientRpc(Managers.Game.Player_DPS, Managers.Game.Other_DPS);
        }
    }
}
