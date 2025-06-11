using Data;
using Unity.Netcode;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public enum EAnimState
    {
        None,
        IDLE,
        MOVE,
        ATTACK,
        DIE,
    }

    protected SpriteRenderer _renderer;
    protected Animator _animator;
    protected NetworkObject _networkObject;

    private static readonly int AnimHashIdle = Animator.StringToHash("IDLE");
    private static readonly int AnimHashMove = Animator.StringToHash("MOVE");
    private static readonly int AnimHashAttack = Animator.StringToHash("ATTACK");
    private static readonly int AnimHashDie = Animator.StringToHash("DIE");

    private EAnimState _currentState = EAnimState.IDLE;

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

    protected void SetAnimState(EAnimState nextState)
    {
        // 만약 현재 상태와 같으면 아무것도 하지 않음 (불필요한 연산 방지)
        if (_currentState == nextState)
            return;

        TurnOffState(_currentState);

        TurnOnState(nextState);

        _currentState = nextState;
    }
    private void TurnOnState(EAnimState state)
    {
        switch (state)
        {
            case EAnimState.IDLE:
                _animator.SetBool(AnimHashIdle, true);
                break;
            case EAnimState.MOVE:
                _animator.SetBool(AnimHashMove, true);
                break;
        }
    }

    private void TurnOffState(EAnimState state)
    {
        switch (state)
        {
            case EAnimState.IDLE:
                _animator.SetBool(AnimHashIdle, false);
                break;
            case EAnimState.MOVE:
                _animator.SetBool(AnimHashMove, false);
                break;
        }
    }

    public void PlayAnimTrigger(EAnimState triggerState)
    {
        switch (triggerState)
        {
            case EAnimState.ATTACK:
                _animator.SetTrigger(AnimHashAttack);
                break;
            case EAnimState.DIE:
                _animator.SetTrigger(AnimHashDie);
                break;
        }
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

    //public void AnimatorChange(string temp, bool Trigger)
    //{
    //    if (Trigger)
    //    {
    //        _animator.SetTrigger(temp);           
    //        return;
    //    }
    //    _animator.SetBool("IDLE", false);
    //    _animator.SetBool("MOVE", false);
    //    _animator.SetBool(temp, true);
    //}
}
