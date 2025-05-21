using Data;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _target;
    private ulong _heroId;

    public void Init(GameObject target, Hero hero)
    {
        _target = target;
        _heroId = hero.NetworkObjectId;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null)
        {
            float distance = Vector2.Distance(transform.position, _target.transform.position);
            if (distance > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, _speed * Time.deltaTime);
            }
            else if (distance <= 0.1f)
            {
                GameObject go = Managers.Resource.Instantiate($"Effects/LongRangeHitEffect");
                go.transform.position = _target.transform.position;
                // Hero가 살아있는지 체크
                if (Net_Utils.TryGetSpawnedObject(_heroId, out var heroObject)) 
                {
                    Hero hero = heroObject.GetComponent<Hero>();
                    if (hero != null)
                    {
                        hero.AttackDamage(_target.GetComponent<NetworkObject>());
                    }
                }
                Managers.Resource.Destroy(this.gameObject);
            }
        }
        else
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}
