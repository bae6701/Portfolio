using Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    private Hero owner = null;
    private Image Fill;
    public bool isReady = false;
    public SkillData skillData;
    public GameObject particle;
    public void SetSpBar(Hero hero)
    {
        owner = hero;
        skillData = hero._heroData.skillData;
        if (skillData.skillType != SkillType.NONE)
        {
            Fill = Utils.FindChild<Image>(gameObject, "Fill_01", true);
            Fill.transform.gameObject.SetActive(true);
            StartCoroutine(SkillDelay());
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (owner._target != null && isReady)
        {
            isReady = false;
            StartCoroutine(SkillDelay());
            GetSkill();
        }
    }

    private void GetSkill()
    {
        switch (skillData.skillType)
        {
            case SkillType.GUN:
                Gun();
                break;
            case SkillType.SHIELD:
                Shield();
                break;
            
        }
    }

    private double SkillDamager()
    {
        return owner._heroData.attack * (skillData.skillDamage / 100);
    }

    private void Gun()
    {
        Vector3 pos = owner._target.transform.position;
        GameObject go = Managers.Resource.Instantiate("Effects/GunParticle");
        go.transform.position = pos;
        float value = Managers.Spawn.DistanceMagnitude * skillData.skillRange;
        for (int i = 0; i < Managers.Spawn.monsters.Count; i++)
        {           
            if (Vector2.Distance(pos, Managers.Spawn.monsters[i].transform.position) <= value+ 0.2f)
            {
                Monster monster = Managers.Spawn.monsters[i];
                monster.GetDamage(SkillDamager());

                monster.ApplyDebuffServerRpc(DebuffType.STUN, HeroType.GUN);
            }
        }
    }

    private void Shield()
    {
        Vector3 pos = transform.parent.position;
        var list = Managers.Spawn.SelectSpawnHeroList(Net_Utils.LocalId() == 0);
        float value = Managers.Spawn.DistanceMagnitude * skillData.skillRange;
        for (int i = 0; i < list.Count; i++)
        {
            Hero hero = list[i];
            if (hero == null) break;
            if (Vector2.Distance(pos, hero.transform.position) <= value + 0.2f)
            {  
                AttackSpeedUp((int)(skillData.skillDamage / 100.0f), skillData.duration, hero);
            }
        }
    }

    Coroutine attackSpeedCoroutine = null;
    public void AttackSpeedUp(int value, float time, Hero hero)
    {
        if (attackSpeedCoroutine != null)
        {
            if (value <= hero.attackSpeedBuff)
            {
                StopCoroutine(attackSpeedCoroutine);
            }
        }
        attackSpeedCoroutine = StartCoroutine(AttackSpeedBuffSet(value, time, hero));
    }
    IEnumerator AttackSpeedBuffSet(int value, float time, Hero hero)
    {
        if (value > hero.attackSpeedBuff)
        {
            hero.attackSpeedBuff = value;
            GameObject go = Managers.Resource.Instantiate("Effects/ShieldParticle");
            go.transform.position = hero.transform.position;
            go.transform.SetParent(hero.transform);
            Destroy(go, time);
        }       
        yield return new WaitForSeconds(time);
        hero.attackSpeedBuff = 1;
    }


    IEnumerator SkillDelay()
    {
        float t = 0.0f;
        float cooltime = owner._heroData.skillData.coolTime;
        while (t < cooltime)
        {
            t += Time.deltaTime;
            Fill.fillAmount = t / cooltime;
            yield return null;
        }
        isReady = true;
    }
    
}
