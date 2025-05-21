using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HeroHolder : NetworkBehaviour
{
    [SerializeField] private Button _composition_Btn;
    [SerializeField] private Button _sell_Btn;
    public HeroType _holderType;
    public int _holderMaxCount;
    public string _holder_Name;
    public int _index;
    private Transform _holderRange;
    private Transform _movePlace;
    private Transform _eventBtns;

    public List<Hero> _heros = new List<Hero>();
    public readonly Vector2[] one =  { Vector2.zero };
    public readonly Vector2[] two = { new Vector2(-0.1f, 0.05f), new Vector2(0.1f, -0.1f) };
    public readonly Vector2[] three = { new Vector2(-0.1f, 0.1f), new Vector2(0.1f, -0.05f), new Vector2(-0.15f, -0.15f) };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _holderMaxCount = 3;
        _holderRange = transform.GetChild(0);
        _movePlace = transform.GetChild(1);
        _eventBtns = transform.GetChild(2);
        _composition_Btn.onClick.AddListener(Composition);
        _sell_Btn.onClick.AddListener(() => Sell());
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(Managers.Spawn.gridX, Managers.Spawn.gridY);
        _movePlace.transform.localScale = collider.size;
    }

    public void Init(Vector2 pos, int index, HeroType holderType)
    {
        transform.position = pos;
        _index = index;
        _holderType = holderType;
    }

    public void Sell(bool Composition = false)
    {
        if (Composition == false)
        {
            string temp = $"{Net_Utils.RarityColor(_heros[_heros.Count - 1]._heroData.rarity)}{_heros[_heros.Count - 1].name}</color>을/를 판매하였습니다.";
            UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
            gameScene.BottomUI.NaviUI.GetNavigation(temp);
        }

        Net_Utils.HostAndClientMethod(
            () => SellServerRpc(Net_Utils.LocalId()),
            () => SellCharacter(Net_Utils.LocalId()));
    }

    [ServerRpc(RequireOwnership = false)]
    private void SellServerRpc(ulong clientId)
    {
        SellCharacter(clientId);
    }

    private void SellCharacter(ulong clientId)
    {
        if (_heros.Count == 0) return;
        Hero hero = _heros[_heros.Count - 1];
        ulong heroNetworkObjectId = hero.NetworkObjectId;
        NetworkObject networkHero = NetworkManager.Singleton.SpawnManager.SpawnedObjects[heroNetworkObjectId];
        SellClientRpc(heroNetworkObjectId, clientId);     
        networkHero.Despawn();
    }

    [ClientRpc]
    private void SellClientRpc(ulong heronetworkobjectId, ulong clientId)
    {
        if (Net_Utils.TryGetSpawnedObject(heronetworkobjectId, out NetworkObject heroNetworkObject))
        {
            Hero hero = heroNetworkObject.GetComponent<Hero>();
            Managers.Spawn.RemoveHero(hero, clientId == 0);
            _heros.Remove(hero);
            if (_heros.Count == 0)
            {
                _holderType = HeroType.NONE;
                HideRange();
            }
            CheckGetPosition();
        }
    }

    public void Composition()
    {
        List<HeroHolder> holders = new List<HeroHolder>();
        holders.Add(this);
        string checkHost = Net_Utils.LocalId() == 0 ? "HostHolder" : "ClientHolder";
        Dictionary<string, HeroHolder> dict = Managers.Spawn.SelectHeroHolder(checkHost);
        foreach (var data in dict)
        {
            if (data.Key.Contains(checkHost))
            { 
                if (data.Value._holderType == HeroType.NONE) break;

                if (data.Value._holderType == _heros[0].GetHeroType() && data.Value != this)
                {
                    holders.Add(data.Value);
                }
            }
        }


        int cnt = 0;
        string[] holderTemp = new string[2];
        bool GetBreak = false;
        for (int i = 0; i < holders.Count; i++)
        {
            for (int j = 0; j < holders[i]._heros.Count; j++)
            {
                if (holders[i]._heros.Count > 0)
                {
                    holderTemp[cnt] = holders[i]._holder_Name;
                    cnt++;
                    if (cnt >= 2)
                    {
                        GetBreak = true;
                        break;
                    }
                }
            }
            if (GetBreak) break;
        }

        for (int i = 0; i < holderTemp.Length; i++)
        {
            if (holderTemp[i] == "" || holderTemp[i] == null)
            {
                Debug.Log("합성에 필요한 유닛이 부족합니다.");
                return;
            }        
        }
        for (int i = 0; i < holderTemp.Length; i++)
        {
            dict[holderTemp[i]].Sell(true);
        }

        Managers.Spawn.Spawner.Summon(Rarity.UNCOMMON, HeroType.NONE);
    }

    public void HeroChange(HeroHolder otherholder)
    {
        List<Vector2> poss = new List<Vector2>();
        switch (_heros.Count)
        {
            case 1:
                poss = new List<Vector2>(one);
                break;
            case 2:
                poss = new List<Vector2>(two);
                break;
            case 3:
                poss = new List<Vector2>(three);
                break;
        }

        for (int i = 0; i < poss.Count; i++)
        {
            Vector2 worldPosition = otherholder.transform.TransformPoint(poss[i]);
            poss[i] = worldPosition;
        }

        for (int i = 0; i < _heros.Count; i++)
        {
            if(IsServer)
                _heros[i].transform.parent = otherholder.transform;
            
            _heros[i].Position_Change(otherholder, poss[i]);
        }
    }

    public void ShowRange()
    {
        float range = _heros[0]._heroData.attackRange * 2;
        _holderRange.localScale = new Vector2(range, range);
        _holderRange.gameObject.SetActive(true);
        _eventBtns.gameObject.SetActive(true);
    }

    public void HideRange()
    {
        _holderRange.gameObject.SetActive(false);
        _eventBtns.gameObject.SetActive(false);
    }

    public void SelectedHolder()
    {
        foreach (var hero in _heros)
        {
            hero.SelectedObject();
        }
    }

    public void UnSelectedHolder()
    {
        foreach (var hero in _heros)
        {
            hero.UnSelectedObject();
        }
    }

    public void SelectMovePlace()
    {
        _movePlace.gameObject.SetActive(true);
    }

    public void UnSelectMovePlace()
    {
        _movePlace.gameObject.SetActive(false);
    }

    public void SpawnCharacter(HeroType heroType, ulong isHost)
    {
        if(IsServer)
            ServerSpawnHeroServerRpc(Net_Utils.LocalId(), heroType, isHost);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerSpawnHeroServerRpc(ulong clientId, HeroType heroType, ulong isHost)
    { 
        HeroSpawn(clientId, heroType, isHost);
    }

    private void HeroSpawn(ulong clientId, HeroType heroType, ulong isHost)
    {
        GameObject go = Managers.Resource.Instantiate("Character/Hero");
        go.transform.position = transform.position;
        Hero hero = go.GetComponent<Hero>();
        hero.NetworkObject.Spawn();
        
        hero.transform.parent = transform;

        ClientSpawnHeroClientRpc(hero.NetworkObjectId, clientId, heroType, isHost);
    }
    
    void CheckGetPosition()
    {
        for (int i = 0; i < _heros.Count; i++)
        { 
            _heros[i].transform.localPosition = Hero_Vectoer_Pos(_heros.Count)[i];
            _heros[i].SetSortingOrder(i + 1);
        }
    }

    private Vector2[] Hero_Vectoer_Pos(int count)
    {
        switch (count)
        {
            case 1: 
                return one;
            case 2: 
                return two;
            case 3: 
                return three;
        }
        return null;
    }

    [ClientRpc]
    private void ClientSpawnHeroClientRpc(ulong networkId, ulong clientId, HeroType heroType, ulong isHost)
    {
        if (Net_Utils.TryGetSpawnedObject(networkId, out NetworkObject heroNetworkObject))
        {
            _holderType = heroType;
            Hero hero = heroNetworkObject.GetComponent<Hero>();         
            _heros.Add(hero);
            Managers.Spawn.AddHero(hero, isHost == clientId);
            hero.Init(heroType, this);
            CheckGetPosition();
        }
    }
}
