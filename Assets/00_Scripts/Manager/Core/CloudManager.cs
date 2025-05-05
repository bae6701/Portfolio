using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;

[Serializable]
public class HeroBox
{
    public int count;
    public int level;

    public HeroBox(int count, int level)
    {
        this.count = count;
        this.level = level;
    }
}
[Serializable]
public class Account
{
    public string playerName;
    public int level;
    public int wave;
    public int energy;
    public int gold;
    public int gem;
    public Dictionary<HeroType, HeroBox> heroData;

    public Account(string playerName, int level, int wave, int energy, int gold, int gem)
    {
        this.playerName = playerName;
        this.level = level;
        this.wave = wave;
        this.energy = energy;
        this.gold = gold;
        this.gem = gem;
        this.heroData = new Dictionary<HeroType, HeroBox>();
        foreach (var value in Managers.Data.heroDict)
        {
            this.heroData.Add(value.Value.heroType, new HeroBox(0, 1));
        }
    }
}

public class CloudManager
{
    private const string playerDataKey = "PlayerData";

    public Account account = null;
    public Account other_account = null;
    public int Wave
    {
        get { return account.wave; }
        set
        {
            if (account.wave.Equals(value)) return;

            account.wave = value;

            Save();
        }
    }  
    public int Energy
    {
        get { return account.energy; }
        set
        {
            if (account.energy.Equals(value)) return;

            account.energy = value;

            Save();
        }
    }
    public int Gold
    {
        get { return account.gold; }
        set
        {
            if (account.gold.Equals(value)) return;

            account.gold = value;

            Save();
        }
    }
    public int Gem
    {
        get { return account.gem; }
        set
        {
            if (account.gem.Equals(value)) return;

            account.gem = value;

            Save();
        }
    }

    public float timer = 0.0f;
    bool isSaving = false;
    public void Update()
    {
        if (!isSaving)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= 10.0f)
            {
                timer = 0.0f;
                Save(() => isSaving = false);
            }
        }
    }

    public async void Save(Action action = null)
    {
        isSaving = true;
        if (account != null)
        {
            await SavePlayerData(account);
        }
        if (action != null)
        {
            action?.Invoke();
        }
    }

    public async Task SavePlayerData(Account account)
    {
        try
        {           
            string jsonData = JsonConvert.SerializeObject(account);
            var data = new Dictionary<string, object> { { playerDataKey, jsonData } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);           
        }
        catch (Exception e)
        {
            Debug.LogError("SavePlayerData Error : " + e.Message);
        }
    }

    public async Task<Account> LoadPlayerAccount()
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { playerDataKey });

            if (savedData.TryGetValue(playerDataKey, out Item item))
            {
                string jsonString = item.Value.GetAs<string>();
                account = JsonConvert.DeserializeObject<Account>(jsonString);

                return account;
            }
            else
            {
                return new Account("", 1, 0, 30, 0, 0);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("LoadPlayerData Error : " + e.Message);
        }

        return null;
    }

    public async Task DeletePlayerData()
    {
        try
        {
            Unity.Services.CloudSave.Models.Data.Player.DeleteOptions options = null;
            await CloudSaveService.Instance.Data.Player.DeleteAsync(playerDataKey, options);
        }
        catch (Exception e)
        {
            Debug.LogError("DeletePlayerData Error : " + e.Message);
        }
    }
}
