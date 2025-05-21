using System.Collections.Generic;
using UnityEngine;

public class SystemBase : MonoBehaviour
{
	public List<WorkerController> Workers = new List<WorkerController>();

	[SerializeField] private Restaurant Owner;

	public virtual bool HasJob => false;

	public virtual void AddWorker(WorkerController wc)
	{
		Workers.Add(wc);
		wc.CurrentSystem = this;
	}

	public virtual void RemoveWorker(WorkerController wc)
	{
		Workers.Remove(wc);
		wc.StopCoroutine(wc.WorkerJob);
		wc.WorkerJob = null;
		wc.CurrentSystem = null;
	}
}
