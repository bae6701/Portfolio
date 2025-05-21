using Data;
using Unity.Netcode;
using UnityEngine;

public class Character : NetworkBehaviour
{
    protected SpriteRenderer _renderer;
    protected Animator _animator;
    protected NetworkObject _networkObject;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void SetSortingOrder(int value)
    {
        _renderer.sortingOrder = value;
    }

    public void SelectedObject()
    {
        _renderer.color = Color.red;
    }
    public void UnSelectedObject()
    {
        _renderer.color = Color.white;
    }   

    public void AnimatorChange(string temp, bool Trigger)
    {
        if (Trigger)
        {
            _animator.SetTrigger(temp);           
            return;
        }
        _animator.SetBool("IDLE", false);
        _animator.SetBool("MOVE", false);
        _animator.SetBool(temp, true);
    }
}
