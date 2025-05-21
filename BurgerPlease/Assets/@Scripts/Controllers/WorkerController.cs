using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WorkerController : StickmanController
{
	protected CharacterController _controller;
	public SystemBase CurrentSystem;

	public Coroutine WorkerJob;
	public void DoJob(IEnumerator job)
	{
		if (WorkerJob != null)
			StopCoroutine(WorkerJob);

		WorkerJob = StartCoroutine(job);
	}

	protected override void Awake()
	{
		base.Awake();

		_controller = GetComponent<CharacterController>();
	}

	private void Start()
	{
		State = Define.EAnimState.Move;
	}

	protected override void Update()
	{
		base.Update();

		if (HasArrivedAtDestination)
		{
			_navMeshAgent.isStopped = true;
			State = Define.EAnimState.Idle;
		}
		else
		{
			State = Define.EAnimState.Move;
		}		
	}
}
