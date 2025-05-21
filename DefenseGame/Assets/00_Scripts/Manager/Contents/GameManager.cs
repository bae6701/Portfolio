using Data;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager
{
    public UI_GameScene GameScene;

    public int Money = 1000;
    public int SummonCount = 20;
    public float Timer = 60.0f;
    public float WaveTimer = 60.0f;
    public int Wave = 1;
    public bool GetBoss = false;

    public int MonsterCount = 0;
    public int MonsterMaxCount = 100;

    public int HeroMaxCount = 25;

    public int GetWaveIndex()
    {
        return (Wave - 1) % 11;
    }
    public bool BossWave()
    {
        return Wave % 10 == 0;
    }

    public double Player_DPS;
    public double Other_DPS;

    public int[] UpGradeLevel = new int[4] { 1, 1, 1, 1};
    public int[] UpGradeLevelMoney = new int[4] { 30, 30, 30, 30};

    public void StartGame(UI_GameScene gameScene)
    {
        GameScene = gameScene;
        Managers.Spawn.SpawnStart();
    }

    public void GetMoney(int value, HostType type = HostType.ALL)
    {
        if (type == HostType.ALL)
        {
            GameScene.BottomUI.NotifyGetMoneyClientRpc(value);
        }
    }

    public void SetDPS(double damage, bool isHost)
    {
        if (isHost)
        {
            Player_DPS += damage;
        }
        else
        {
            Other_DPS += damage;
        }
    }

    public string WavePoint()
    {
        int min = Mathf.FloorToInt(Timer / 60);
        int sec = Mathf.FloorToInt(Timer % 60);

        return $"{min:00} : {sec:00}";
    } 
}
