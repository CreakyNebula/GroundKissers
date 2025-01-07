using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using TMPro;

public class Selector : MonoBehaviour
{
    public float moveSpeed = 100f;     // Velocidad de movimiento
    private Vector2 moveInput;
    private PlayerInput playerInput;

    public int playerIndex;           // Índice del jugador
    private bool pressed;             // Para manejar la confirmación de selección

    public string sceneToLoad;        // Escena a cargar tras la selección
    public Color selectedColor; // Prefab seleccionado por este jugador
    public GameObject instantiatedPrefab;

    private GameManager gameManagerScript;
    private bool hasLogged = false;
    private PlayerConfigurationMenu playerConfigurationMenu;
    public TMP_Text text;

    private void Awake()
    {
        playerInput = transform.parent.GetComponent<PlayerInput>();
    }

    private void Start()
    {
        playerIndex = playerInput.playerIndex;
        transform.parent.transform.parent = GameObject.Find("PlayerConfigurationManager").transform;
        playerConfigurationMenu = GameObject.Find("PlayerConfigurationManager").GetComponent<PlayerConfigurationMenu>();

        text = transform.GetChild(0).GetComponent<TMP_Text>();
        int mando = playerIndex + 1;
        string text1 = "P" + mando.ToString();
        text.text = text1;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        // Verifica si estamos en la escena específica y si el mensaje no ha sido registrado aún
        if (SceneManager.GetActiveScene().name == sceneToLoad && !hasLogged)
        {
            hasLogged = true; // Marca que el mensaje ya fue mostrado
            PlayerInput playerInput = transform.parent.GetComponent<PlayerInput>();
            playerConfigurationMenu.Respawn(playerInput);
            Destroy(transform.parent.gameObject);

        }
        // Movimiento del selector en la UI
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * moveSpeed * Time.deltaTime;
        transform.localPosition += movement;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Button") )
        {
            Debug.Log("toy dentro");
            if(pressed)
            {
                // Obtener el script asociado al botón
                Player_Chosen buttonScript = collision.GetComponent<Player_Chosen>();


                if (buttonScript != null)
                {
                    selectedColor = buttonScript.color;
                    playerConfigurationMenu.playerColor[playerIndex] = selectedColor;
                    text.color= buttonScript.color;
                    Debug.Log("aprete");
                }
            }
            
        }
    }


    public void Selected(InputAction.CallbackContext callbackContext)
    {
        

        // Detecta la confirmación de seleccióna
        if (callbackContext.performed) pressed = true;
        else if (callbackContext.canceled) pressed = false;
    }

    // Método para obtener el prefab seleccionado de un jugador
   
}
