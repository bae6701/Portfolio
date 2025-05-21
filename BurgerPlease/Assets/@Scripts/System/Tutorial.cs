using System.Collections;
using UnityEngine;

public enum ETutorialState
{ 
	None,
	CreateFirstTable,
	CreateBurgerMachine,
	CreateCounter,
	PickupBurger,
	PutBurgerOnCounter,
	SellBurger,
	CleanTable,
	CreateSecendTable,
	CreateOffice,

	Done,
}

public class Tutorial : MonoBehaviour
{
	[SerializeField] private MainCounterSystem _mainCounterSystem;
	private RestaurantData _data;

	private ETutorialState _state
	{
		get { return _data.TutorialState; }
		set { _data.TutorialState = value; }
	}

	public void SetInfo(RestaurantData data)
	{
		_data = data;

		if (_state == ETutorialState.None)
			_state = ETutorialState.CreateFirstTable;

		StartCoroutine(CoStartTutorial());
	}

	IEnumerator CoStartTutorial()
	{
		yield return new WaitForEndOfFrame();

		Counter counter = _mainCounterSystem.Counter;
		Grill grill = _mainCounterSystem.Grill;
		Table firstTable = _mainCounterSystem.Tables[0];
		Table secondTable = _mainCounterSystem.Tables[1];
		Office office = _mainCounterSystem.Office;
		TrashCan trashCan = _mainCounterSystem.TrashCan;

		counter.SetUnlockedState(EUnlockedState.Hidden);
		grill.SetUnlockedState(EUnlockedState.Hidden);
		firstTable.SetUnlockedState(EUnlockedState.Hidden);
		secondTable.SetUnlockedState(EUnlockedState.Hidden);
		office.SetUnlockedState(EUnlockedState.Hidden);

		grill.StopSpawnBurger = true;

		if (_state == ETutorialState.CreateFirstTable)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create First Table");

			firstTable.SetUnlockedState(EUnlockedState.ProcessiongConstruction);
			yield return new WaitUntil(() => firstTable.IsUnlocked);
			_state = ETutorialState.CreateBurgerMachine;
		}

		firstTable.SetUnlockedState(EUnlockedState.Unlocked);

		if (_state == ETutorialState.CreateBurgerMachine)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create BurgerMachine");

			grill.SetUnlockedState(EUnlockedState.ProcessiongConstruction);
			yield return new WaitUntil(() => grill.IsUnlocked);
			_state = ETutorialState.CreateCounter;
		}

		grill.SetUnlockedState(EUnlockedState.Unlocked);

		if (_state == ETutorialState.CreateCounter)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create Counter");

			counter.SetUnlockedState(EUnlockedState.ProcessiongConstruction);
			yield return new WaitUntil(() => counter.IsUnlocked);
			_state = ETutorialState.PickupBurger;
		}

		counter.SetUnlockedState(EUnlockedState.Unlocked);
		grill.StopSpawnBurger = false;

		if (_state == ETutorialState.PickupBurger)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Pickup Burger");

			yield return new WaitUntil(() => grill.CurrentWorker != null);
			_state = ETutorialState.PutBurgerOnCounter;
		}

		if (_state == ETutorialState.PutBurgerOnCounter)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Put Burger On Counter");

			yield return new WaitUntil(() => counter.CurrentBurgerWorker != null);
			_state = ETutorialState.SellBurger;
		}

		if (_state == ETutorialState.SellBurger)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Sell Burger");

			yield return new WaitUntil(() => firstTable.TableState == Define.ETableState.Reserved);
			_state = ETutorialState.CleanTable;
		}

		if (_state == ETutorialState.CleanTable)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("");

			yield return new WaitUntil(() => firstTable.TableState == Define.ETableState.Dirty);

			GameManager.Instance.GameSceneUI.SetToastMessage("Clean Table");

			yield return new WaitUntil(() => firstTable.TableState != Define.ETableState.Dirty);

			yield return new WaitUntil(() => trashCan.CurrentWorker != null);
			_state = ETutorialState.CreateSecendTable;
		}

		if (_state == ETutorialState.CreateSecendTable)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create Second Table");

			secondTable.SetUnlockedState(EUnlockedState.ProcessiongConstruction);
			yield return new WaitUntil(() => secondTable.IsUnlocked);
			_state = ETutorialState.CreateOffice;
		}

		secondTable.SetUnlockedState(EUnlockedState.Unlocked);

		if (_state == ETutorialState.CreateOffice)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create Office");

			office.SetUnlockedState(EUnlockedState.ProcessiongConstruction);
			yield return new WaitUntil(() => office.IsUnlocked);
			_state = ETutorialState.Done;
		}

		office.SetUnlockedState(EUnlockedState.Unlocked);

		GameManager.Instance.GameSceneUI.SetToastMessage("");

		yield return null;
	}
}
