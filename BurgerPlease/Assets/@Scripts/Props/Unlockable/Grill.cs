using System.Collections;
using UnityEngine;

// 1. 패티 애니메이션 (OK)
// 2. 햄버거 주기적으로 생성 (OK)
// 3. [Collider] 길찾기 막기 (OK)
// 4. Burger Pile (OK)
// 5. [Trigger] 햄버거 영역 안으로 들어오면, 플레이어가 갖고감.
public class Grill : UnlockableBase
{
	private BurgerPile _burgers;
	private WorkerInteraction _interaction;

	public int BurgerCount => _burgers.ObjectCount;
	public WorkerController CurrentWorker => _interaction.CurrentWorker;
	public Transform WorkerPos;
	public bool StopSpawnBurger = false;

	protected void Awake()
	{
		_burgers = Utils.FindChild<BurgerPile>(gameObject);

		_interaction = _burgers.GetComponent<WorkerInteraction>();
		_interaction.InteractInterval = 0.2f;
		_interaction.OnInteraction = OnBurgerInteraction;
	}
	Coroutine _coSpawnBurger;

	private void OnEnable()
	{
		if (_coSpawnBurger != null)
			StopCoroutine(_coSpawnBurger);

		_coSpawnBurger = StartCoroutine(CoSpawnBurgers());
	}

	private void OnDisable()
	{
		if (_coSpawnBurger != null)
			StopCoroutine(_coSpawnBurger);

		_coSpawnBurger = null;
	}

	IEnumerator CoSpawnBurgers()
	{
		while (true)
		{
			yield return new WaitUntil(() => _burgers.ObjectCount < Define.GRILL_MAX_BURGER_COUNT);

			if(StopSpawnBurger == false)
				_burgers.SpawnObject();

			yield return new WaitForSeconds(Define.GRILL_SPAWN_BURGER_INTERVAL);
		}
	}

	void OnBurgerInteraction(WorkerController pc)
	{
		// 쓰레기 운반 상태에선 안됌
		if (pc.Tray.CurrentTrayObjectType == Define.EObjectType.Trash)
			return;

		_burgers.PileToTray(pc.Tray);
	}
}
