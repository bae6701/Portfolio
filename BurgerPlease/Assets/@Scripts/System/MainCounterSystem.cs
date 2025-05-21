using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class MainCounterSystem : SystemBase
{
	public Grill Grill;
	public Counter Counter;
	public List<Table> Tables = new List<Table>();
	public TrashCan TrashCan;
	public Office Office;

	public WorkerController[] Jobs = new WorkerController[(int)EMainCounterJob.MaxCount];
	public override bool HasJob
	{ 
		get
		{
			for (int i = 0; i < (int)EMainCounterJob.MaxCount; i++)
			{
				EMainCounterJob type = (EMainCounterJob)i;
				if (ShouldDoJob(type))
					return true;
			}

			return false;
		}
	}

	private void Awake()
	{
		Counter.Owner = this;
	}

	private void Update()
	{
		foreach (WorkerController worker in Workers)
		{
			if (worker.WorkerJob != null)
				continue;

			IEnumerator job = DoMainCounterWorkerJob(worker);
			worker.DoJob(job);
		}
	}

	#region Worker
	public override void AddWorker(WorkerController wc)
	{
		base.AddWorker(wc);
	}

	bool ShouldDoJob(EMainCounterJob jobType)
	{
		WorkerController wc = Jobs[(int)jobType];
		if (wc != null)
			return false;

		switch (jobType)
		{
			case EMainCounterJob.MoveBurger:
				{
					if (Grill == null)
						return false;
					if (Grill.CurrentWorker != null)
						return false;
					if (Grill.BurgerCount == 0)
						return false;
					if (Counter.NeedMoreBurgers == false)
						return false;

					return true;
				}
			case EMainCounterJob.CounterCashier:
				{
					if (Counter == null)
						return false;
					if (Counter.CurrentCashierWorker != null)
						return false;
					if (Counter.NeedCashier == false)
						return false;
					if (Counter.FindTableToServeGuests() == null)
						return false;

					return true;
				}
			case EMainCounterJob.CleanTable:
				{
					foreach (Table table in Tables)
					{
						if (table.TableState == ETableState.Dirty)
							return true;
					}
					return false;
				}
		}
		return false;
	}

	IEnumerator DoMainCounterWorkerJob(WorkerController wc)
	{
		while (true)
		{
			yield return new WaitForSeconds(1);

			bool foundJob = false;

			if (ShouldDoJob(EMainCounterJob.MoveBurger))
			{
				foundJob = true;
				// 일감 점유
				Jobs[(int)EMainCounterJob.MoveBurger] = wc;

				// 그릴로 이동.
				wc.SetDestination(Grill.WorkerPos.position, ()=>
				{
					wc.transform.rotation = Grill.WorkerPos.rotation;
				});

				// 가는중
				yield return new WaitUntil(() => wc.HasArrivedAtDestination);

				// 그릴 도착했으면 일정 시간 대기
				wc.transform.rotation = Grill.WorkerPos.rotation;
				yield return new WaitForSeconds(3);

				// 햄버거 수집했다면 카운터로 이동
				wc.SetDestination(Counter.BurgerWorkerPos.position, ()=>
				{
					wc.transform.rotation = Counter.BurgerWorkerPos.rotation;
				});

				// 가는중
				yield return new WaitUntil(() => wc.HasArrivedAtDestination);

				// 카운터 도착했으면 일정시간 대기
				wc.transform.rotation = Counter.BurgerWorkerPos.rotation;
				yield return new WaitForSeconds(2);

				// 일감 점유 해제
				Jobs[(int)EMainCounterJob.MoveBurger] = null;
			}

			if (ShouldDoJob(EMainCounterJob.CounterCashier))
			{
				foundJob = true;

				// 일감 점유
				Jobs[(int)EMainCounterJob.CounterCashier] = wc;

				// 계산대로 이동
				wc.SetDestination(Counter.CashierWorkerPos.position);

				// 가는중
				yield return new WaitUntil(() => wc.HasArrivedAtDestination);

				// 계산대 도착했으면 일정 시간 대기
				wc.transform.rotation = Counter.CashierWorkerPos.rotation;
				yield return new WaitUntil(() => Counter.FindTableToServeGuests() == null);

				// 일감 점유 해제
				Jobs[(int)EMainCounterJob.CounterCashier] = null;
			}

			if (ShouldDoJob(EMainCounterJob.CleanTable))
			{
				Table table = Tables.Where(t => t.TableState == ETableState.Dirty).FirstOrDefault();
				if (table == null)
					continue;

				foundJob = true;

				// 일감 점유
				Jobs[(int)EMainCounterJob.CleanTable] = wc;

				// 테이블로 이동
				wc.SetDestination(table.WorkerPos.position, ()=>
				{
					wc.transform.rotation = table.WorkerPos.rotation;
				});

				// 가는중
				yield return new WaitUntil(() => wc.HasArrivedAtDestination);

				// 테이블 도착했으면 일정 시간 대기
				wc.transform.rotation = table.WorkerPos.rotation;
				yield return new WaitUntil(() => table.TableState != ETableState.Dirty);

				//쓰레기통으로 이동
				wc.SetDestination(TrashCan.WorkerPos.position, () =>
				{
					wc.transform.rotation = TrashCan.WorkerPos.rotation;
				});

				// 쓰레기통 도착했으면 일정 시간 대기
				wc.transform.rotation = table.WorkerPos.rotation;
				yield return new WaitUntil(() => wc.IsServing == false);

				Jobs[(int)EMainCounterJob.CleanTable] = null;
			}

			if (foundJob == false)
			{
				RemoveWorker(wc);
			}
		}
	}

	public bool HasEmptyCleanTable()
	{
		foreach (Table table in Tables)
		{
			if (table.TableState == ETableState.None)
				return true;
		}

		return false;
	}

	#endregion
}
