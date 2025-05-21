using Data;
using System;
using Unity.Netcode;
using UnityEngine;

public class Net_Utils
{
    public static ulong LocalId()
    {
        return NetworkManager.Singleton.LocalClientId;
    }

    public static void HostAndClientMethod(Action clientAction, Action ServerAction)
    {
        if (NetworkManager.Singleton.IsClient)
        {
            clientAction?.Invoke();
        }
        else if (NetworkManager.Singleton.IsServer)
        { 
            ServerAction?.Invoke();
        }
    }
    public static bool CheckHost(ulong clientId)
    {
        return LocalId() == clientId;
    }

    public static bool TryGetSpawnedObject(ulong networkObjectId, out NetworkObject spawnedObject)
    {
        return NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out spawnedObject);
    }

    public static string RarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.COMMON: return "<color=#A4A4A4>";
            case Rarity.UNCOMMON: return "<color=#79FF73>";
            case Rarity.RARE: return "<color=#6EE5FF>";
            case Rarity.HERO: return "<color=#FF9EF5>";
            case Rarity.LEGENDARY: return "<color=#FFBA13>";
        }
        return "";
    }

    public static Color RarityCircleColor(Rarity rarity)
    {
        string temp = RarityColor(rarity);
        int start = temp.IndexOf("#");
        int end = temp.IndexOf(">", start);

        string hex = temp.Substring(start, end - start);
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        return color;
    }

    public static HeroType RandomsSpawn(Rarity rarity)
    {
        UnityEngine.Random.InitState((int)(DateTime.Now.Ticks));
        HeroType heroType = HeroType.NONE;
        switch (rarity)
        {
            case Rarity.COMMON:
                heroType = (HeroType)UnityEngine.Random.Range((int)HeroType.ARCHER, (int)HeroType.GUN);
                break;
            case Rarity.UNCOMMON:
                heroType = (HeroType)UnityEngine.Random.Range((int)HeroType.GUN, (int)HeroType.HAMMER);
                break;
            case Rarity.RARE:
                heroType = (HeroType)UnityEngine.Random.Range((int)HeroType.HAMMER, (int)HeroType.WITCH + 1);
                break;
            case Rarity.HERO:
                Debug.Log("영웅 등급이 없습니다.");
                break;
            case Rarity.LEGENDARY:
                Debug.Log("레전더리 등급이 없습니다.");
                break;
        }
        return heroType;
    }
}
