using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using Data;

public class SpawnManager
{
    public Spawner Spawner;
   
    #region Data
    public Dictionary<string, HeroHolder> SelectHeroHolder(string key)
    {
        bool isPlayer;
        if (key.Contains("Host"))
        {
            isPlayer = Net_Utils.LocalId() == 0 ? true : false;
        }
        else
        {
            isPlayer = Net_Utils.LocalId() == 0 ? false : true;
        }
        return isPlayer ? player_heroHolders : other_heroHolders;
    }
    public Dictionary<string, HeroHolder> SelectHeroHolder(ulong clientId) { return Net_Utils.CheckHost(clientId) ? player_heroHolders : other_heroHolders; }
    public Dictionary<string, HeroHolder> player_heroHolders = new Dictionary<string, HeroHolder>();
    public Dictionary<string, HeroHolder> other_heroHolders = new Dictionary<string, HeroHolder>();

    public int[] heroHolderCount = new int[2];

    public List<Vector2> SelectSpawnMonsterList(ulong clientId) { return Net_Utils.CheckHost(clientId) ? player_Monster_Move_List : other_Monster_Move_List; }
    public List<Vector2> player_Monster_Move_List = new List<Vector2>();
    public List<Vector2> other_Monster_Move_List = new List<Vector2>();

    public List<Vector2> SelectSpawnList(string key) 
    {
        bool isPlayer;
        if (key.Contains("Host"))
        {
            isPlayer = Net_Utils.LocalId() == 0 ? true : false;
        }
        else
        {
            isPlayer = Net_Utils.LocalId() == 0 ? false : true;
        }
        return isPlayer ? player_Spawn_List : other_Spawn_List;
    }
    public List<Vector2> SelectSpawnList(ulong clientId) { return Net_Utils.CheckHost(clientId) ? player_Spawn_List : other_Spawn_List; }
    public List<Vector2> player_Spawn_List = new List<Vector2>();
    public List<Vector2> other_Spawn_List = new List<Vector2>();

    public List<bool> SelectSpawnListArray(string key) 
    {
        bool isPlayer;
        if (key.Contains("Host"))
        {
            isPlayer = Net_Utils.LocalId() == 0 ? true : false;
        }
        else
        {
            isPlayer = Net_Utils.LocalId() == 0 ? false : true;
        }

        return isPlayer ? player_Spawn_List_Array : other_Spawn_List_Array; 
    }
    public List<bool> SelectSpawnListArray(ulong clientId) { return Net_Utils.CheckHost(clientId) ? player_Spawn_List_Array : other_Spawn_List_Array; }
    public List<bool> player_Spawn_List_Array = new List<bool>();
    public List<bool> other_Spawn_List_Array = new List<bool>();

    public List<Monster> monsters = new List<Monster>();
    public List<Monster> bossMonsters = new List<Monster>();

    public List<Hero> SelectSpawnHeroList(bool isHost) { return isHost ? host_heros : client_heros; }
    public List<Hero> host_heros = new List<Hero>();
    public List<Hero> client_heros = new List<Hero>();

    #endregion

    // 3 * 6 Size field
    public int xCount = 3;
    public int yCount = 6;
    public int GetFieldMaxCount() { return (xCount * yCount); }
    public float gridX, gridY;
    public float DistanceMagnitude;

    public void SpawnStart()
    {
        GameObject go = Managers.Resource.Instantiate("Map/Spawner");
        Spawner = Utils.GetOrAddComponent<Spawner>(go);      
    }

    public void AddHeroHolder(HeroHolder heroholder, string Key, ulong clientId)
    {
        heroholder._holder_Name = Key + heroHolderCount[clientId].ToString();
        heroHolderCount[clientId]++;
        SelectHeroHolder(clientId).Add(heroholder._holder_Name, heroholder);
    }

    public void AddHero(Hero hero, bool isHost)
    {
        var heros = SelectSpawnHeroList(isHost);
        heros.Add(hero);
    }

    public void RemoveHero(Hero hero, bool isHost)
    {
        var heros = SelectSpawnHeroList(isHost);
        heros.Remove(hero);
    }

    public void AddMonster(Monster monster)
    {
        if (monster._monsterType == MonsterType.BOSS)
        {
            bossMonsters.Add(monster);
        }
        else
        { 
            monsters.Add(monster);
        }
        Managers.Game.MonsterCount++;
        if (Managers.Game.MonsterCount >= Managers.Game.MonsterMaxCount)
        {
            if (Managers.Cloud.Wave <= Managers.Game.Wave)
            {
                Managers.Spawn.Spawner.SaveWaveCountClientRpc();
                //Managers.Cloud.Wave = Managers.Game.Wave;
            }
            Managers.Game.GameScene.MiddleUI.GameOverClientRpc();
        }
        Managers.Spawn.Spawner.NotifyClientMonsterCountClientRpc(Managers.Game.MonsterCount);
    }

    
    public void RemoveMonster(Monster monster)
    {
        if (monster._monsterType == MonsterType.BOSS)
        {
            bossMonsters.Remove(monster);
            if (bossMonsters.Count == 0)
            {
                Managers.Game.GetBoss = false;
                Managers.Game.Timer = 0.0f;
            }
        }
        else
        {
            monsters.Remove(monster);
        }
        Managers.Game.MonsterCount--;
        Managers.Spawn.Spawner.NotifyClientMonsterCountClientRpc(Managers.Game.MonsterCount);
    }
}
