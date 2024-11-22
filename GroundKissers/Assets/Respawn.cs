using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject playerPrefab; // Prefab del jugador que se instanciará
    private GameObject playerInScene; // Referencia al jugador en la escena
    public float deadTime = 5f; // Tiempo de espera antes de reaparecer
    private float deadTimeCounter; // Contador interno para el tiempo
    private bool isFirstSpawn = true; // Bandera para verificar si es la primera vez

    void Start()
    {
        deadTimeCounter = deadTime; // Inicializa el contador al tiempo especificado
        RespawnPlayer(); // Realiza el primer spawn inmediatamente
    }

    void Update()
    {
        // Comprueba si el jugador no está en la escena
        if (playerInScene == null)
        {
            // Cuenta el tiempo
            deadTimeCounter -= Time.deltaTime;

            // Si ha pasado el tiempo necesario, reaparece al jugador
            if (deadTimeCounter <= 0f)
            {
                RespawnPlayer();
                deadTimeCounter = deadTime; // Reinicia el contador para futuros usos
            }
        }
        else
        {
            // Si el jugador está en la escena, reinicia el contador
            deadTimeCounter = deadTime;
        }
    }

    // Método para reaparecer al jugador
    void RespawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(0, 0, 0); // Ajusta según lo necesario

        // Instancia el jugador y actualiza la referencia
        playerInScene = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        // Si es la primera vez, reinicia la bandera para asegurar la lógica
        if (isFirstSpawn)
        {
            isFirstSpawn = false; // Marcar que ya no es el primer spawn
        }
    }
}
