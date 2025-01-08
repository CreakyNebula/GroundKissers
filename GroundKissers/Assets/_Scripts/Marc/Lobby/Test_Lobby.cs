using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Net;

public class Test_Lobby : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartBeatTimer;
    private string playerName;


    public int ManualMaxPlayers;
    public string ManualLobbyCode;
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);

        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Baeturia" + UnityEngine.Random.Range(10, 99);
        Debug.Log(playerName);
    }
    private void Update()
    {
        HandleLobbyHeartBeat();
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CreateLobby();
        }
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            ListLobbies();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            JoinLobbyByCode(ManualLobbyCode);
        }
        if(Input.GetKeyDown(KeyCode.RightShift))
        {
            QuickJoinLobby();
        }

    }

    private async void HandleLobbyHeartBeat()
    {
        if(hostLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;

            if(heartBeatTimer < 0)
            {
                float heartBeatTimerMax = 15;
                heartBeatTimer = heartBeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    private async void CreateLobby()
    {
        try
        {

            string lobbyName = "MyLobby" + playerName;
            int maxPlayers = ManualMaxPlayers;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers,createLobbyOptions);

            hostLobby=lobby;
            joinedLobby=lobby;
            Debug.Log("Created Lobby" + lobby.Name + "" + lobby.MaxPlayers + " " + lobby.LobbyCode);
            PrintPlayer(hostLobby);
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void ListLobbies()
    {
        try
        {
            //Filtros
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
            {
                //si tiene mas de(GT) 0 available slots hace lista de 25 lobbys
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                //Es en orden descendente de creacion (false) 
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false,QueryOrder.FieldOptions.Created)
                }

            };


            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found" + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + "" + lobby.MaxPlayers +" " + lobby.LobbyCode);

            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);

        }

    }

    private async void QuickJoinLobby()
    {
        try
        {
           await Lobbies.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions()
            {
                Player = GetPlayer()
            };
            Lobby lobb = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobb;
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName) }
                    }
        };
    }

    private void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }
    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby " + lobby.Name);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + "" + player.Data["PlayerName"].Value);
        }
    }
}
