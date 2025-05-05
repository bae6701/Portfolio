using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

public partial class NetManager
{
    public async void JoinGameWithCode(string inputJoinCode)
    {
        if (string.IsNullOrEmpty(inputJoinCode))
        {
            Debug.Log("유효하지 않은 Join Code입니다.");
            return;
        }
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(inputJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
                );

            StartClient();
            Debug.Log("Join Code로 게임에 접속 성공!");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("게임 접속 실패 : " + e);
        }
    }

    public async void StartMatchmaking(Action<string> res)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("로그인되지 않았습니다.");
            return;
        }

        currentLobby = await FindAvailableLobby();

        if (currentLobby == null)
        {
            await CreateNewLobby(res);
        }
        else
        {
            await JoinLobby(currentLobby.Id);
        }
    }

    private async Task<Lobby> FindAvailableLobby()
    {
        try
        {
            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            if (queryResponse.Results.Count > 0)
            {
                return queryResponse.Results[0];
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("로비 찾기 실패 " + e);
        }
        return null;
    }

    private async Task CreateNewLobby(Action<string> res)
    {
        try
        {
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤 매치방", maxPlayers);
            Debug.Log("새로운 방 생성됨 : " + currentLobby.Id);

            await AllocateRelayServerAndJoin(currentLobby);
            res.Invoke(currentLobby.Id);
            StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("로비 생성 실패 " + e);
        }
    }

    public async void DestroyLobby(string lobbyId)
    {
        try
        {
            if (!string.IsNullOrEmpty(lobbyId))
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
                Debug.Log("Lobby destroyed : " + lobbyId);
            }
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to destroy lobby : " + e.Message);
        }
    }

    private async Task JoinLobby(string lobbyId)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string joinCode = currentLobby.Data["joinCode"].Value;

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
                );

            StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("로비 참가 실패 " + e);
        }
    }

    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4, // Relay 서버 IP 주소
                (ushort)allocation.RelayServer.Port, // Relay 서버 포트 번호
                allocation.AllocationIdBytes, // 할당된 Allocation ID
                allocation.Key, // 암호화 키
                allocation.ConnectionData, // 자신의 연결 데이터
                allocation.ConnectionData); // 호스트의 연결 데이터

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
                }
            });

            //JoinCodeText.text = joinCode;
            Debug.Log("Relay 서버 할당 완료. Join Code : " + joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Relay 서버 할당 실패 : " + e);
        }
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("호스트가 시작되었습니다.");

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnHostDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        OnPlayerJoined();
    }

    private void OnHostDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnHostDisconnected;
        }
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("클라이언트가 연결되었습니다.");
    }

    public void OnPlayerJoined()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers)
        {
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
    }
}
