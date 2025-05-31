using Data;
using UnityEngine;

public class Boss : Monster
{
    protected override void Init()
    {
        base.Init();
        AddBar();
        AddStunEffect();
    }

    protected override void AddBar()
    {
        GameObject go = Managers.Resource.Instantiate("Contents/BossHpBar");
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(0, 0.3f, 0);
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
    }

    protected override void AddStunEffect()
    {
        _stunParticle = Managers.Resource.Instantiate("Contents/Stun").GetComponent<ParticleSystem>();
        _stunParticle.transform.SetParent(transform);
        _stunParticle.transform.position = new Vector3(0.06f, 0.3f, 0);
        _stunParticle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        _stunParticle.gameObject.SetActive(false);
    }
}
