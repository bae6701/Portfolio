using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

public partial class NetManager
{
    private Lobby currentLobby;

    private const int maxPlayers = 2;

    public async Task Init(UI_Loading loading = null)
    {
        float displayProgress = 0f;
        var initTask = UnityServices.InitializeAsync();
        while (!initTask.IsCompleted)
        {
            displayProgress = Mathf.MoveTowards(displayProgress, 0.3f, Time.deltaTime);
            loading?.SetProgress(displayProgress);
            await Task.Yield();
        }
        displayProgress = 0.3f;
        loading?.SetProgress(displayProgress);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            var authTask = AuthenticationService.Instance.SignInAnonymouslyAsync();
            while (!authTask.IsCompleted)
            {
                displayProgress = Mathf.MoveTowards(displayProgress, 0.6f, Time.deltaTime);
                loading?.SetProgress(displayProgress);
                await Task.Yield();
            }
            displayProgress = 0.6f;
            loading?.SetProgress(displayProgress);
            var loadData = Managers.Cloud.LoadPlayerAccount();
            while (!loadData.IsCompleted)
            {
                displayProgress = Mathf.MoveTowards(displayProgress, 1f, Time.deltaTime);
                loading?.SetProgress(displayProgress);
                await Task.Yield();
            }
            Managers.Cloud.account = loadData.Result;
            loading?.SetProgress(1f);
        }
    }    
}
