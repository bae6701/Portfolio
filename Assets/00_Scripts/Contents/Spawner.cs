using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{   
    Coroutine spawn_Monster_Coroutine;

    public void Start()
    {
        LoadMap();
        if(IsClient)
            spawn_Monster_Coroutine = StartCoroutine(Spawn_Monster_Coroutine());
    }

    public void Clear()
    {
        foreach (var obj in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (obj != null && obj.IsSpawned)
            {
                obj.Despawn(true);
            }
        }
    }

    #region LoadMap
    public void LoadMap()
    {
        Transform playerfield = gameObject.transform.GetChild(0);
        Transform otherfield = gameObject.transform.GetChild(1);
        if (playerfield == null || otherfield == null)
        {
            Debug.Log($"Failed to get field");
            return;
        }

        List<Vector2> player_Monster_Move_List = Managers.Spawn.player_Monster_Move_List;
        List<Vector2> other_Monster_Move_List = Managers.Spawn.other_Monster_Move_List;

        for (int i = 0; i < playerfield.childCount - 1; i++) // -1은 자식에 있는 BackGround제외
        {
            player_Monster_Move_List.Add(playerfield.GetChild(i).position);
        }

        for (int i = 0; i < otherfield.childCount - 1; i++) // -1은 자식에 있는 BackGround제외
        {
            other_Monster_Move_List.Add(otherfield.GetChild(i).position);
        }

        Grid_Start(playerfield, true);
        Grid_Start(otherfield, false);
    }

    private void Grid_Start(Transform tf, bool Player)
    {
        int yCount = Managers.Spawn.yCount;
        int xCount = Managers.Spawn.xCount;

        float gridSizeX = tf.localScale.x / yCount;
        float gridSizeY = tf.localScale.y / xCount;

        Managers.Spawn.gridX = gridSizeX;
        Managers.Spawn.gridY = gridSizeY;
        Managers.Spawn.DistanceMagnitude = Mathf.Max(gridSizeX, gridSizeY);

        for (int row = 0; row < xCount; row++)
        {
            for (int col = 0; col < yCount; col++)
            {
                float xPos = (-tf.localScale.x / 2) + (col * gridSizeX) + (gridSizeX / 2) + tf.localPosition.x;
                float yPos = (Player ? 1 : -1) * ((tf.localScale.y / 2) - (row * gridSizeY) - (gridSizeY / 2)) + tf.localPosition.y;

                switch (Player)
                {
                    case true:
                        Managers.Spawn.player_Spawn_List.Add(new Vector2(xPos, yPos));
                        Managers.Spawn.player_Spawn_List_Array.Add(false);
                        break;
                    case false:
                        Managers.Spawn.other_Spawn_List.Add(new Vector2(xPos, yPos));
                        Managers.Spawn.other_Spawn_List_Array.Add(false);
                        break;
                }
                SpawnHolders(Player);
            }
        }
        Managers.Spawn.heroHolderCount[0] = 0;
        Managers.Spawn.heroHolderCount[1] = 0;
    }
    #endregion

    #region Holder Spawn

    public void SpawnHolders(bool Player)
    {
        if (IsServer)
            StartCoroutine(DelayHeroHolderSpawn(Player));
    }

    IEnumerator DelayHeroHolderSpawn(bool Player)
    {
        GameObject go = Managers.Resource.Instantiate("Character/HERO_HOLDER");
        NetworkObject networkobj = Utils.GetOrAddComponent<NetworkObject>(go);
        networkobj.Spawn();
        ulong clientId = (ulong)(Player == true ? 0 : 1);
        string checkHost = Player ? "HostHolder" : "ClientHolder";
        int value = Player ? 0 : 1;

        Managers.Spawn.heroHolderCount[value]++;

        yield return new WaitForSeconds(0.5f);

        SpawnGridHolder_ClientRpc(networkobj.NetworkObjectId, clientId, checkHost);
    }

    [ClientRpc]
    private void SpawnGridHolder_ClientRpc(ulong networkId, ulong clientId, string key)
    {
        if (Net_Utils.TryGetSpawnedObject(networkId, out NetworkObject heroNetworkObject))
        {
            List<bool> spawnCheckList = Managers.Spawn.SelectSpawnListArray(key);
            List<Vector2> spawnPosList = Managers.Spawn.SelectSpawnList(key);

            int checkIndex = spawnCheckList.IndexOf(false);
            if (checkIndex != -1)
            {
                spawnCheckList[checkIndex] = true;
                HeroHolder holder = heroNetworkObject.GetComponent<HeroHolder>();
                holder.Init(spawnPosList[checkIndex], checkIndex, HeroType.NONE);
                Managers.Spawn.AddHeroHolder(holder, key, clientId);
            }
        }
    }
    #endregion

    #region Character Spawn

    

    public void Summon(Rarity rarity, HeroType heroType)
    {
        if (heroType == HeroType.NONE)
        {
            heroType = Net_Utils.RandomsSpawn(rarity);
        }
        // 필드가 가득 차있지 않다면 스폰
        //if (Managers.Spawn.SelectHeroHolder(Net_Utils.LocalId()).Count <= Managers.Spawn.GetFieldMaxCount() - 1)
        //{
        Net_Utils.HostAndClientMethod(
            () => {
                ServerSpawnHero_ServerRpc(Net_Utils.LocalId(), rarity, heroType);
            },
            () => {
                FindHeroHolder(Net_Utils.LocalId(), rarity, heroType);
                });
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerSpawnHero_ServerRpc(ulong clientId, Rarity rarity, HeroType heroType)
    {
        FindHeroHolder(clientId, rarity, heroType);
    }

    private void FindHeroHolder(ulong clientId, Rarity rarity, HeroType heroType)
    {     
        string checkHost = Net_Utils.CheckHost(clientId) ? "HostHolder" : "ClientHolder";
        HeroHolder findHolder = FindSpawnHeroHoder(checkHost, heroType, Managers.Spawn.SelectHeroHolder(clientId));

        if (findHolder != null)
        {
            HeroData data = Managers.Data.heroDict[heroType];
            string temp = $"{Net_Utils.RarityColor(rarity)}{data.name}</color>을/를 획득하였습니다.";
            Managers.Game.GameScene.BottomUI.ClientNavigationClientRpc(clientId, temp);
            findHolder.SpawnCharacter(heroType, clientId);
            return;
        }
        else
        {
            Debug.Log("필드가 가득 찼습니다.");
        }
    }

    //소환될 위치를 랜덤 Type으로 찾고 반환
    public Vector3 HeroSpawnPos(HeroType heroType)
    {      
        string checkHost = Net_Utils.LocalId() == 0 ? "HostHolder" : "ClientHolder";
        HeroHolder findHolder = FindSpawnHeroHoder(checkHost, heroType, Managers.Spawn.SelectHeroHolder(checkHost));

        if (findHolder == null) return Vector3.zero;

        return findHolder.transform.position;
    }

    

    public void HolderPositionChange(HeroHolder holder1, HeroHolder holder2, ulong clientId)
    {
        Net_Utils.HostAndClientMethod(
            () => HolderPositionSet_ServerRpc(holder1._holder_Name, holder2._holder_Name, clientId),
            () => HolderPositionSet(holder1._holder_Name, holder2._holder_Name, clientId));
    }

    [ServerRpc(RequireOwnership = false)]
    public void HolderPositionSet_ServerRpc(string holder1, string holder2, ulong clientId)
    {
        HolderPositionSet(holder1, holder2, clientId);
    }

    private void HolderPositionSet(string key1, string key2, ulong clientId)
    {
        HolderPositionSet_ClientRpc(key1, key2, clientId);
    }
    [ClientRpc]
    private void HolderPositionSet_ClientRpc(string key1, string key2, ulong clientId)
    {
        var holders = Managers.Spawn.SelectHeroHolder(clientId);
        HeroHolder holder1 = holders[key1];
        HeroHolder holder2 = holders[key2];
        holder1.HeroChange(holder2);
        holder2.HeroChange(holder1);

        List<Hero> temp = holder1._heros;
        holder1._heros = holder2._heros;
        holder2._heros = temp;

        HeroType tempType = holder1._holderType;
        holder1._holderType = holder2._holderType;
        holder2._holderType = tempType;
    }

    // 정해진 Type으로 Holder 찾기
    public HeroHolder FindSpawnHeroHoder(string checkHost, HeroType heroType, Dictionary<string, HeroHolder> dict)
    {
        HeroHolder emptyHolder = null;

        foreach (var kvp in dict)
        {
            if (!kvp.Key.Contains(checkHost))
                continue;

            HeroHolder holder = kvp.Value;
            if (holder._holderMaxCount > holder._heros.Count)
            {
                if (holder._heros.Count > 0)
                {
                    var firstHero = holder._heros[0];

                    if (firstHero.GetHeroType() == heroType)
                    {
                        return holder;
                    }
                }
                else if (emptyHolder == null)
                {
                    emptyHolder = holder;
                    emptyHolder._holderType = heroType;
                }
            }            
        }

        return emptyHolder;
    }
    #endregion

    #region Monster Spawn

    [ClientRpc]
    public void SaveWaveCountClientRpc()
    {
        Managers.Cloud.Wave = Managers.Game.Wave;
        Debug.Log(Managers.Cloud.Wave);
    }

    IEnumerator Spawn_Monster_Coroutine(int count = 40)
    {
        float time = Managers.Game.BossWave() ? 5.0f :  1f;
        yield return new WaitForSeconds(time);

        Net_Utils.HostAndClientMethod(
            () => ServerMonsterSpawnServerRpc(Net_Utils.LocalId()),
            () => MonsterSpawn(Net_Utils.LocalId()));

        if (Managers.Game.BossWave()) yield break;
        if (count == 0) yield break;
        count--;
        spawn_Monster_Coroutine = StartCoroutine(Spawn_Monster_Coroutine(count));
    }
    

    public void BossSpawn()
    {
        if (spawn_Monster_Coroutine != null)
        {
            StopCoroutine(spawn_Monster_Coroutine);
        }
        StartCoroutine(Spawn_Monster_Coroutine());
    }

    public void ReSpawnMob()
    {
        if (spawn_Monster_Coroutine != null)
        {
            StopCoroutine(spawn_Monster_Coroutine);
        }
        spawn_Monster_Coroutine = StartCoroutine(Spawn_Monster_Coroutine());
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerMonsterSpawnServerRpc(ulong clientId)
    {
        MonsterSpawn(clientId);
    }
    private void MonsterSpawn(ulong clientId)
    {
        GameObject go = null;
        Monster monster = null;
        if (Managers.Game.BossWave())
        {
            go = Managers.Resource.Instantiate(Managers.Data.bossDict[0].prefabPath);           
            if (go == null) return;
            monster = go.GetComponent<Boss>();
        }
        else
        {
            go = Managers.Resource.Instantiate(Managers.Data.monsterDict[Managers.Game.GetWaveIndex()].prefabPath);
            if (go == null) return;
            monster = go.GetComponent<Mob>();
        }
        
        if (monster == null) return; 

        if (monster is Boss)
            monster._monsterType = MonsterType.BOSS;
        else if(monster is Mob)
            monster._monsterType = MonsterType.MOB;

        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();
        Managers.Spawn.AddMonster(monster);
        ClientMonsterSetClientRpc(monster.NetworkObjectId, clientId);
    }

    [ClientRpc]
    public void NotifyClientMonsterCountClientRpc(int count)
    {
        Managers.Game.MonsterCount = count;
    }

    [ClientRpc]
    private void ClientMonsterSetClientRpc(ulong networkObjectId, ulong clientId)
    {
        if (Net_Utils.TryGetSpawnedObject(networkObjectId, out NetworkObject monsterNetworkObject))
        {
            MonsterData monsterData = null;
            Monster monster = null;
            if (Managers.Game.BossWave())
            {
                monsterData = Managers.Data.bossDict[0];
                monster = Utils.GetOrAddComponent<Boss>(monsterNetworkObject.gameObject);
            }
            else
            {
                monsterData = Managers.Data.monsterDict[Managers.Game.GetWaveIndex()];
                monster = Utils.GetOrAddComponent<Mob>(monsterNetworkObject.gameObject);
            }

            List<Vector2> spawnList = Managers.Spawn.SelectSpawnMonsterList(clientId);

            if (spawnList == null || spawnList.Count == 0)
            {
                Debug.LogError($"[MonsterSpawn] 클라이언트 {clientId}에 대한 몬스터 리스트가 비어있습니다.");
                return; 
            }

            monster.MonsterData = monsterData;
            monster.Init(spawnList);
        }
    }
    #endregion
}
