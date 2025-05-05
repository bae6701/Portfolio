using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
   public BaseScene CurrentScene { get { return GameObject.FindAnyObjectByType<BaseScene>(); } }


    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(GetSceneNameByType(type), LoadSceneMode.Single);
        }
    }

    string GetSceneNameByType(Define.Scene type)
    {
        string name = Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
