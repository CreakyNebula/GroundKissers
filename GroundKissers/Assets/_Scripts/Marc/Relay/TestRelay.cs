using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

public class TestRelay : MonoBehaviour
{
    public static TestRelay Instance { get; private set; }
       private void Awake() {
        	Instance = this;
    	}
    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId) ;
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation,"dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            return joinCode;    
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }
    private void Update()
    {
       
    }

    [SerializeField]private string inspJoinCode;
    public async void JoinRelay(string joincode)
    {
        try 
        {
            Debug.Log("Joining Relay with  " + joincode);
            JoinAllocation joinAllocation =await Relay.Instance.JoinAllocationAsync(joincode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);


            NetworkManager.Singleton.StartClient();
            GameObject canvas = GameObject.Find("SuperLobbyCanvas");
            canvas.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
