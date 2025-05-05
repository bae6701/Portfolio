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
            Debug.Log("��ȿ���� ���� Join Code�Դϴ�.");
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
            Debug.Log("Join Code�� ���ӿ� ���� ����!");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("���� ���� ���� : " + e);
        }
    }

    public async void StartMatchmaking(Action<string> res)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("�α��ε��� �ʾҽ��ϴ�.");
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
            Debug.Log("�κ� ã�� ���� " + e);
        }
        return null;
    }

    private async Task CreateNewLobby(Action<string> res)
    {
        try
        {
            currentLobby = await LobbyService.Instance.CreateLobbyAsync("���� ��ġ��", maxPlayers);
            Debug.Log("���ο� �� ������ : " + currentLobby.Id);

            await AllocateRelayServerAndJoin(currentLobby);
            res.Invoke(currentLobby.Id);
            StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("�κ� ���� ���� " + e);
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
            Debug.Log("�κ� ���� ���� " + e);
        }
    }

    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4, // Relay ���� IP �ּ�
                (ushort)allocation.RelayServer.Port, // Relay ���� ��Ʈ ��ȣ
                allocation.AllocationIdBytes, // �Ҵ�� Allocation ID
                allocation.Key, // ��ȣȭ Ű
                allocation.ConnectionData, // �ڽ��� ���� ������
                allocation.ConnectionData); // ȣ��Ʈ�� ���� ������

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
                }
            });

            //JoinCodeText.text = joinCode;
            Debug.Log("Relay ���� �Ҵ� �Ϸ�. Join Code : " + joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Relay ���� �Ҵ� ���� : " + e);
        }
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("ȣ��Ʈ�� ���۵Ǿ����ϴ�.");

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
        Debug.Log("Ŭ���̾�Ʈ�� ����Ǿ����ϴ�.");
    }

    public void OnPlayerJoined()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers)
        {
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
    }
}
