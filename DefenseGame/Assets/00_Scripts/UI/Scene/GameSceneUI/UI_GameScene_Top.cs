using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_GameScene_Top : UI_Base
{   
    public UI_Wave WaveUI { get; private set; }

    [SerializeField] private TextMeshProUGUI player_name;
    [SerializeField] private TextMeshProUGUI other_name;
    [SerializeField] private TextMeshProUGUI player_level;
    [SerializeField] private TextMeshProUGUI other_level;
    public override void Init()
    {
        WaveUI = Utils.FindChild<UI_Wave>(this.gameObject, "Wave");      
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetNickNameOtherPlayer();
    }

    private void Update()
    {
        if (IsServer)
        {
            bool changeWave = false;
            if (Managers.Game.Timer > 0)
            {
                Managers.Game.Timer -= Time.deltaTime;
                Managers.Game.Timer = Mathf.Max(Managers.Game.Timer, 0);
            }
            else
            {
                if (Managers.Game.GetBoss)
                {
                    return;
                }
                changeWave = true;
                Managers.Game.Wave++;
                Managers.Game.Timer = Managers.Game.WaveTimer;
            }
            NotifyClientRpc(Managers.Game.Timer, Managers.Game.Wave, changeWave);
        }
    }
    public void SetNickNameOtherPlayer()
    {
        string name = Managers.Cloud.account.playerName;
        int level = Managers.Cloud.account.level;
        if (IsServer)
        {           
            SendNickNameToClientRpc(name, level);
        }
        else
        {
            SendNickNameToServerRpc(name, level);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendNickNameToServerRpc(string name, int level)
    {
        if (Managers.Cloud.other_account == null)
        {
            Managers.Cloud.other_account = new Account(name, level, 0, 0, 0, 0);
        }
        Managers.Cloud.other_account.playerName = name;
        Managers.Cloud.other_account.level = level;
        SetPlayerUI();
    }

    [ClientRpc]
    private void SendNickNameToClientRpc(string name, int level)
    {
        if (!IsServer)
        {
            if (Managers.Cloud.other_account == null)
            {
                Managers.Cloud.other_account = new Account(name, level, 0, 0, 0, 0);
            }
            Managers.Cloud.other_account.playerName = name;
            Managers.Cloud.other_account.level = level;
            SetPlayerUI();
        }        
    }

    private void SetPlayerUI()
    {
        player_name.text = Managers.Cloud.account.playerName;
        other_name.text = Managers.Cloud.other_account.playerName;
        player_level.text = "Lv. " + Managers.Cloud.account.level;
        other_level.text = "Lv. " + Managers.Cloud.other_account.level;
    }

    [ClientRpc]
    public void NotifyClientRpc(float timer, int wave, bool changeWave)
    {
        Managers.Game.Timer = timer;
        Managers.Game.Wave = wave;

        if (changeWave)
        {
            if (Managers.Game.BossWave())
            {
                Managers.Game.GetBoss = true;
                Managers.Spawn.Spawner.BossSpawn();
                WaveUI.BossWaveBarOpen();
            }
            else
            {
                WaveUI.WaveNotiBarOpen(Managers.Game.Wave.ToString());
                Managers.Spawn.Spawner.ReSpawnMob();
            }
        }
    }
}
