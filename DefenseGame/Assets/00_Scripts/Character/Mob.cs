using Data;
using UnityEngine;

public class Mob : Monster
{
    protected override void Init()
    {
        base.Init();
        AddBar();
        AddStunEffect();
    } 

    protected override void AddBar()
    {
        GameObject go = Managers.Resource.Instantiate("Contents/MonsterHpBar");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
    }
    protected override void AddStunEffect()
    {
        _stunParticle = Managers.Resource.Instantiate("Contents/Stun").GetComponent<ParticleSystem>();
        _stunParticle.transform.SetParent(transform);
        _stunParticle.transform.position = Vector3.zero;
        _stunParticle.gameObject.SetActive(false);
    }   
}
