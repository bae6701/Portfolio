using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static bool _init = false;
	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null && _init == false)
			{
				_init = true;
				_instance = (T)FindAnyObjectByType(typeof(T));

				if (_instance == null)
				{
					GameObject go = new GameObject($"@{typeof(T).Name}");
					_instance = go.AddComponent<T>();					
				}
			}

			return _instance;
		}
	}
}
