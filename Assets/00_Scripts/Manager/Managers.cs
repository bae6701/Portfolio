using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    SpawnManager _spawn = new SpawnManager();
    GameManager _game = new GameManager();

    public static SpawnManager Spawn { get { return Instance._spawn; } }
    public static GameManager Game { get { return Instance._game; } }
    #endregion

    #region Core
    DataManager _data = new DataManager();
    UIManager _ui = new UIManager();
    ResourceManager _resource = new ResourceManager();
    NetManager _net = new NetManager();
    SceneManagerEx _scene = new SceneManagerEx();
    PoolManager _pool= new PoolManager();
    CloudManager _cloud = new CloudManager();
    InAppManager _inapp = new InAppManager();

    public static DataManager Data { get { return Instance._data; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static NetManager Net { get { return Instance._net; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static CloudManager Cloud { get { return Instance._cloud; } }
    public static InAppManager Inapp { get { return Instance._inapp; } }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    private void Update()
    {
        Cloud.Update();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._inapp.Init();
        }
    }

    public static void Clear()
    {
        UI.Clear();
        if (Spawn.Spawner != null)
            Spawn.Spawner.Clear();
    }
}
