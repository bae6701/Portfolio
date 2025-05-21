using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Restaurant : MonoBehaviour
{
	public List<SystemBase> RestuarantSystems = new List<SystemBase>();

	public int StageNum = 0;
	public List<UnlockableBase> Props = new List<UnlockableBase>();
	public List<WorkerController> Workers = new List<WorkerController>();

	private RestaurantData _data;

	private void OnEnable()
	{
		GameManager.Instance.AddEventListener(Define.EEventType.HireWorker, OnHireWorker);
		StartCoroutine(CoDistributeWorkerAI());
	}

	private void OnDisable()
	{
		GameManager.Instance.RemoveEventListener(Define.EEventType.HireWorker, OnHireWorker);
	}
	public void SetInfo(RestaurantData data)
	{
		_data = data;

		RestuarantSystems = GetComponentsInChildren<SystemBase>().ToList();
		Props = GetComponentsInChildren<UnlockableBase>().ToList();

		for (int i = 0; i < Props.Count; i++)
		{
			UnlockableStateData stateData = data.UnlockableStates[i];
			Props[i].SetInfo(stateData);
		}

		Tutorial tutorial = GetComponent<Tutorial>();
		if (tutorial != null)
			tutorial.SetInfo(data);

		for (int i = 0; i < data.WorkerCount; i++)
			OnHireWorker();
	}

	void OnHireWorker()
	{
		GameObject go = GameManager.Instance.SpawnWorker();
		WorkerController wc = go.GetComponent<WorkerController>();
		go.transform.position = Define.WORKER_SPAWN_POS;

		Workers.Add(wc);

		_data.WorkerCount = Mathf.Max(_data.WorkerCount, Workers.Count);
	}

	IEnumerator CoDistributeWorkerAI()
	{
		while (true)
		{
			yield return new WaitForSeconds(1);

			yield return new WaitUntil(() => Workers.Count > 0);

			foreach (WorkerController worker in Workers)
			{
				if (worker.CurrentSystem != null)
					continue;

				foreach (SystemBase system in RestuarantSystems)
				{
					if (system.HasJob)
					{
						system.AddWorker(worker);
					}
				}
			}
		}
	}
}
