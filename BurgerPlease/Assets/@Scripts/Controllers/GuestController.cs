using UnityEngine;
using UnityEngine.AI;
using static Define;

public class GuestController : StickmanController
{
	private EGuestState _guestState = EGuestState.None;
	public EGuestState GuestState
	{
		get { return _guestState; }
		set 
		{ 
			_guestState = value;

			if (value == EGuestState.Eating)
				State = EAnimState.Eating;

			UpdateAnimation(); 
		}
	}

	public int CurrentDestQueueIndex;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Update()
	{
		base.Update();

		if (GuestState != EGuestState.Eating)
		{
			if (HasArrivedAtDestination)
			{
				State = EAnimState.Idle;
			}
			else
			{
				State = EAnimState.Move;
			}
		}
	}
}
