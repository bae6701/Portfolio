using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hero : Character
{
    public HeroHolder _parentHolder = null;
    public Monster _target;
    public LayerMask _enemyLayer;
    public HeroData _heroData = new HeroData();

    SpriteRenderer circleRenderer = null;
    public double GetAttackDamage()
    {
        double upgradeValue = (Managers.Game.UpGradeLevel[UpgradeCount()]) * 0.1f;
        return  _heroData.attack * (1 + upgradeValue);
    }
    public float CurrentAttackSpeed
    {
        get { return _heroData.currentattackSpeed; }
        set { _heroData.currentattackSpeed = value; }
    }
    public float AttackSpeed
    {
        get { return _heroData.attackSpeed; }
        set { _heroData.attackSpeed = value; }
    }
    private Skill skill;
    public float attackSpeedBuff = 1;
    public HeroType GetHeroType() { return _heroData.heroType; }
    public bool _isMove = false;
    float stunProbability = 0.5f;
    float slowProbability = 0.5f;

    private int UpgradeCount()
    {
        switch (_heroData.rarity)
        {
            case Rarity.COMMON:
            case Rarity.UNCOMMON:
            case Rarity.RARE:
                return 0;
            case Rarity.HERO:
                return 1;
            case Rarity.LEGENDARY:
                return 2;
        }
        return -1;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Init()
    {
        base.Init();
        circleRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isMove) return;
        CheckForEnmies();
    }

    protected virtual void AddBar() 
    {
        GameObject go = Managers.Resource.Instantiate("Contents/SkillGauge");
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(-0.02f, 0.7f, 0.0f);
        skill = go.GetComponent<Skill>();
        skill.SetSpBar(this);
    }

    public void Init(HeroType type, HeroHolder parent)
    {
        _parentHolder = parent;
        _heroData = Managers.Data.heroDict[type];
        name = _heroData.name;
        GameObject go = Managers.Resource.Instantiate("Effects/SpawnEffect");
        go.transform.position = _parentHolder.transform.position;
        _animator.runtimeAnimatorController = _heroData.animator;
        circleRenderer.color = Net_Utils.RarityCircleColor(_heroData.rarity);
        _heroData.currentattackSpeed = _heroData.attackSpeed;
        AddBar();
    }

    public void Position_Change(HeroHolder holder, Vector2 pos)
    {
        _isMove = true;
        AnimatorChange("MOVE", false);

        _parentHolder = holder;

        //if(IsServer)
        //    transform.parent = holder.transform;

        int sign = (int)Mathf.Sign(pos.x - transform.position.x);

        switch (sign)
        {
            case -1: 
                _renderer.flipX = true;
                break;
            case 1:
                _renderer.flipX = false;
                break;
        }

        StartCoroutine(Move_Coroutine(pos));
    }

    private IEnumerator Move_Coroutine(Vector2 endPos)
    {
        float current = 0.0f;
        float percent = 0.0f;
        Vector2 start = transform.position;
        Vector2 end = endPos;
        while (percent < 1.0f)
        {
            current += Time.deltaTime;
            percent = current / 0.5f;
            Vector2 LerpPos = Vector2.Lerp(start, end, percent);
            transform.position = LerpPos;
            yield return null;
        }
        _isMove = false;
        AnimatorChange("IDLE", false);
        _renderer.flipX = true;
    }
    public void AnimAttackSpeedReturn()
    {
        _animator.speed = 1.0f;
    }

    void CheckForEnmies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(_parentHolder.transform.position, (Managers.Spawn.DistanceMagnitude * _heroData.attackRange), _enemyLayer);
        float attackCoolTime = 1.0f / (AttackSpeed * attackSpeedBuff);
        if (enemiesInRange.Length > 0)
        {
            _target = enemiesInRange[0].GetComponent<Monster>();
            if (CurrentAttackSpeed >= attackCoolTime)
            {
                CurrentAttackSpeed = 0;              
                _animator.speed = AttackSpeed * attackSpeedBuff;
                AttackEffectClientRpc(_target.NetworkObjectId);
            }
        }
        else
        {
            _target = null;
        }
        CurrentAttackSpeed += Time.deltaTime;
    }
    [ClientRpc]
    private void AttackEffectClientRpc(ulong networkobjectid)
    {
        if (Net_Utils.TryGetSpawnedObject(networkobjectid, out NetworkObject monsterNetworkObject))
        {
            if (_target.gameObject == monsterNetworkObject.gameObject)
            {
                if (_heroData.heroAttackType == HeroAttackType.LONGRANGE)
                {
                    GameObject go = Managers.Resource.Instantiate($"Effects/LongRangeProjectile");
                    go.transform.position = transform.position;
                    go.GetOrAddComponent<Projectile>().Init(monsterNetworkObject.gameObject, this);
                }
                else
                {
                    GameObject go = Managers.Resource.Instantiate($"Effects/MeleeHitEffect");
                    go.transform.position = monsterNetworkObject.transform.position;
                    Managers.Resource.Destroy(go, 2.5f);
                    AttackDamage(monsterNetworkObject);
                }
                AnimatorChange("ATTACK", true);
            }           
        }       
    }

    public void AttackDamage(NetworkObject networkobject)
    {
        if (networkobject != null)
        {
            if (IsServer)
            {
                if (_parentHolder._holder_Name.Contains("Host"))
                {
                    Managers.Game.SetDPS(GetAttackDamage(), true);
                }
                else
                {
                    Managers.Game.SetDPS(GetAttackDamage(), false);
                }

                AttackMonsterServerRpc(networkobject.NetworkObjectId);
                if (UnityEngine.Random.value <= slowProbability)
                {
                    networkobject.GetComponent<Monster>().ApplyDebuffServerRpc(DebuffType.SLOW, _heroData.heroType);
                }

                if (UnityEngine.Random.value <= stunProbability)
                {
                    networkobject.GetComponent<Monster>().ApplyDebuffServerRpc(DebuffType.STUN, _heroData.heroType);
                }
            }                     
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackMonsterServerRpc(ulong monsterId)
    {
        if (Net_Utils.TryGetSpawnedObject(monsterId, out var spawnedObject))
        {
            if (spawnedObject != null)
            {
                Monster monster = spawnedObject.GetComponent<Monster>();
                if (monster != null)
                    monster.GetDamage(GetAttackDamage());
            }         
        }
    } 
}
