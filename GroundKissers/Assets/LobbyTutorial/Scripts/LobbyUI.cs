using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {


    public static LobbyUI Instance { get; private set; }


    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;

    [SerializeField] private Button changeRedButton;
    [SerializeField] private Button changeBlueButton;
    [SerializeField] private Button changeGreenButton;
    [SerializeField] private Button changeYellowButton;

    [SerializeField] private Button leaveLobbyButton;



    private void Awake() {
        Instance = this;

        playerSingleTemplate.gameObject.SetActive(false);

        changeRedButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Red);
        });
        changeBlueButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Blue);
        });
        changeGreenButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Green);
        });

        changeYellowButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Yellow);
        });

        leaveLobbyButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
        });


    }

    [SerializeField] private Color[] buttonColors; // Array de colores
    [SerializeField] private Button[] colorButtons; // Array para los botones de cambio de color

    private void Start()
    {
        // Asegúrate de que el array de colores coincide con el array de botones
        if (buttonColors.Length != colorButtons.Length)
        {
            Debug.LogError("El número de colores no coincide con el número de botones.");
            return;
        }

        // Cambia el color de los botones
        for (int i = 0; i < colorButtons.Length; i++)
        {
            Image buttonImage = colorButtons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = buttonColors[i];
            }
        }

        // Suscripción a eventos
        changeRedButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Red);
        });
        changeBlueButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Blue);
        });
        changeGreenButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Green);
        });
        changeYellowButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Yellow);
        });

        leaveLobbyButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
        });

        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;

        Hide();
    }


    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) {
        ClearLobby();
        Hide();
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e) {
        UpdateLobby();
    }

    private void UpdateLobby() {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby) {
        ClearLobby();

        foreach (Player player in lobby.Players) {
            Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
            playerSingleTransform.gameObject.SetActive(true);
            LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

            lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                LobbyManager.Instance.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
            );

            lobbyPlayerSingleUI.UpdatePlayer(player);
        }

        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        

        Show();
    }

    private void ClearLobby() {
        foreach (Transform child in container) {
            if (child == playerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}