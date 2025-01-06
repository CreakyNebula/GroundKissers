using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Selector : MonoBehaviour
{
    public float moveSpeed = 100f;     // Velocidad de movimiento
    private Vector2 moveInput;
    private PlayerInput playerInput;

    public int playerIndex;           // �ndice del jugador
    private bool pressed;             // Para manejar la confirmaci�n de selecci�n

    public string sceneToLoad;        // Escena a cargar tras la selecci�n
    public GameObject selectedPrefab; // Prefab seleccionado por este jugador


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        playerIndex = playerInput.playerIndex;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        // Movimiento del selector en la UI
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * moveSpeed * Time.deltaTime;
        transform.localPosition += movement;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Button") && pressed)
        {
            // Obtener el script asociado al bot�n
            Player_Chosen buttonScript = collision.GetComponent<Player_Chosen>();

            if (buttonScript != null)
            {
                // Asignar el prefab seleccionado para este jugador
                selectedPrefab = buttonScript.playerPrefab;

                // Guardar los datos del jugador en PlayerPrefs
                PlayerPrefs.SetString($"Player{playerIndex}_Prefab", selectedPrefab.name);
                PlayerPrefs.SetInt($"Player{playerIndex}_DeviceID", playerInput.devices[0].deviceId);

                // Guardar el n�mero total de jugadores (solo una vez)
                PlayerPrefs.SetInt("PlayerCount", PlayerInput.all.Count);

                // Cambiar a la escena de juego
                
            }
        }
    }


    public void Selected(InputAction.CallbackContext callbackContext)
    {
        // Detecta la confirmaci�n de selecci�n
        if (callbackContext.performed) pressed = true;
        else if (callbackContext.canceled) pressed = false;
    }


}
