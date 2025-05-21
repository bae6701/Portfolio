using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Character
{   
    MonsterData _monsterData = new MonsterData();
    public virtual MonsterData MonsterData
    {
        get { return _monsterData; }
        set
        {
            if (_monsterData.Equals(value))
                return;

            _monsterData.monsterName = value.monsterName;
            _monsterData.monsterType = value.monsterType;
            _monsterData.prefabPath = value.prefabPath;

            Stat = value.statInfo;
        }
    }
    StatInfo _statInfo = new StatInfo();
    public virtual StatInfo Stat
    {
        get { return _statInfo; }
        set
        {
            if (_statInfo.Equals(value))
                return;

            _statInfo.hp = value.hp;
            _statInfo.maxHp = value.maxHp;
            _statInfo.speed = value.speed;
            _statInfo.currentSpeed = value.currentSpeed;
            _statInfo.defence = value.defence;
            _statInfo.gold = value.gold;
        }
    }

    public float Speed
    {
        get { return Stat.currentSpeed; }
        set { Stat.currentSpeed = value; }
    }
    public virtual double Hp
    {
        get { return Stat.hp; }
        set
        {
            Stat.hp = value;
            UpdateHpBar();
        }
    }

    public MonsterType _monsterType = MonsterType.NONE;
    public List<Vector2> monster_Move_list = new List<Vector2>();
    protected HpBar _hpBar = null;
    protected ParticleSystem _stunParticle = null;
    int target_Value = 0;
    public bool isDead = false;
    public bool isStun = false;
    float currentSlowAmount;
    float currentSlowDuration;

    protected override void Init()
    {
        base.Init();        
    }

    public override void OnNetworkSpawn()
    {
        
    }

    public void Init(List<Vector2> moveList)
    {
        transform.position = moveList[0];
        monster_Move_list = moveList;
        Hp = CalculateMonsterHp(Managers.Game.Wave);
        _statInfo.maxHp = _statInfo.hp;
        _statInfo.currentSpeed = _statInfo.speed;
    }

    protected virtual void AddBar() { }

    protected virtual void AddStunEffect() { }

    protected virtual void UpdateHpBar()
    {
        if (_hpBar == null)
            return;

        float ratio = 0.0f;
        if (Stat.hp > 0)
            ratio = ((float)Hp) / (float)Stat.maxHp;

        _hpBar.SetHpBar(ratio);
    }

    double CalculateMonsterHp(int wave)
    {
        double powerMultiplier = Mathf.Pow(1.1f, wave);

        if (wave % 10 == 0)
        {
            powerMultiplier += 0.05f * (wave / 10);
        }

        return _statInfo.hp * powerMultiplier * ((_monsterData.monsterType == MonsterType.BOSS) ? 10 : 1);
    }
    
    void Update()
    {               
        if (isDead) return;
        if (isStun) return;
        Move();
    }
    protected virtual void Move()
    {
        if (monster_Move_list.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, monster_Move_list[target_Value], Time.deltaTime * _statInfo.currentSpeed);
            if (Vector2.Distance(transform.position, monster_Move_list[target_Value]) <= 0.0f)
            {
                _renderer.flipX = target_Value >= 2 ? true : false;
                target_Value = (target_Value + 1) % 4;
            }
        }      
    }

    public void GetDamage(double dmg)
    {
        if (!IsServer) return;
        if (isDead) return;
        GetDamageMonster(dmg);
        NotifyClientUpdateClientRpc(_statInfo.hp -= dmg, dmg);
    }
    private void GetDamageMonster(double dmg)
    {
        Hp -= dmg;

        GameObject go = Managers.Resource.Instantiate("Effects/HitText", transform);
        HitText hitText = Utils.GetOrAddComponent<HitText>(go);
        hitText.Init(dmg);

        if (_statInfo.hp <= 0)
        {   
            isDead = true;
            gameObject.layer = LayerMask.NameToLayer("Default");          
            StartCoroutine(Dead_Coroutine());
            Managers.Game.GetMoney(_statInfo.gold);
            AnimatorChange("DIE", true);
        }
    }

    [ClientRpc]
    public void NotifyClientUpdateClientRpc(double hp, double dmg)
    {
        Hp = hp;
        
        GameObject go = Managers.Resource.Instantiate("Effects/HitText", transform);
        HitText hitText = Utils.GetOrAddComponent<HitText>(go);
        hitText.Init(dmg);

        if (_statInfo.hp <= 0)
        {
            isDead = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            StartCoroutine(Dead_Coroutine());
            AnimatorChange("DIE", true);
        }
    }

    IEnumerator Dead_Coroutine()
    {
        float Alpha = 1.0f;

        while (_renderer.color.a > 0.0f)
        {
            Alpha -= Time.deltaTime;
            _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, Alpha);

            yield return null;
        }

        if (IsServer)
        {
            Managers.Spawn.RemoveMonster(this);
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void ApplyDebuffServerRpc(DebuffType debuffType, HeroType type)
    {
        if (Net_Utils.TryGetSpawnedObject(NetworkObjectId, out NetworkObject monsterObject))
        {
            if (monsterObject != null)
            {
                DebuffData debuffData = Managers.Data.heroDict[type].debuffData;
                switch (debuffType)
                {
                    case DebuffType.SLOW:
                        if (debuffData.slowAmount > currentSlowAmount || (debuffData.slowAmount == currentSlowAmount && debuffData.slowduration > currentSlowDuration))
                        {
                            currentSlowAmount = debuffData.slowAmount;
                            currentSlowDuration = debuffData.slowduration;

                            ApplySlowClientRpc(currentSlowAmount, currentSlowDuration);
                        }
                        break;
                    case DebuffType.STUN:
                        ApplyDebuffClientRpc(debuffData.stunduration);
                        break;
                }
            }
        }
    }

    [ClientRpc]
    private void ApplyDebuffClientRpc(float duration)
    {
        if (Net_Utils.TryGetSpawnedObject(NetworkObjectId, out NetworkObject monsterObject))
        {
            if (monsterObject != null)
            {
                if (stunCoroutine != null)
                    StopCoroutine(stunCoroutine);

                stunCoroutine = StartCoroutine(EffectCoroutine(duration, () =>
                {
                    isStun = true;
                    if (_stunParticle != null)
                    {
                        _stunParticle.gameObject.SetActive(true);
                        _stunParticle.Play();
                    }                   
                }, () =>
                {
                    isStun = false;
                    if (_stunParticle != null)
                    {
                        _stunParticle.Stop();
                        _stunParticle.gameObject.SetActive(false);
                    }
                }));
            }
        }
    }

    [ClientRpc]
    private void ApplySlowClientRpc(float amount, float duration)
    {
        if (Net_Utils.TryGetSpawnedObject(NetworkObjectId, out NetworkObject monsterObject))
        {
            if (monsterObject != null)
            {
                if (slowCoroutine != null)
                    StopCoroutine(slowCoroutine);

                slowCoroutine = StartCoroutine(EffectCoroutine(duration, () =>
                {
                    float originalSpeed = _statInfo.currentSpeed;
                    float slowSpeed = originalSpeed - (originalSpeed * amount);
                    slowSpeed = Mathf.Max(slowSpeed, 0.1f);

                    _statInfo.currentSpeed = slowSpeed;
                    _renderer.color = Color.blue;
                }, () =>
                {
                    _statInfo.currentSpeed = _statInfo.speed;
                    _renderer.color = Color.white;
                }));
            }           
        }
    }
    Coroutine stunCoroutine;
    Coroutine slowCoroutine;
    private IEnumerator EffectCoroutine(float duration, Action startAction, Action endAction)
    {      
        startAction.Invoke();
        yield return new WaitForSeconds(duration);
        endAction.Invoke();
    }

    
}
