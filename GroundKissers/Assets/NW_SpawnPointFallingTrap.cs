using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NW_SpawnPointFallingTrap : NetworkBehaviour
{
    [SerializeField] private GameObject[] prefabsToSpawn;
    [SerializeField] private float cooldownTime = 5f; // Tiempo de espera después de spawnear
    [SerializeField] private bool isOnCooldown = false;

    private GameObject selectedPrefab;

    public bool IsOnCooldown { get => isOnCooldown; set => isOnCooldown = value; }

  
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer) // Solo el servidor controla la lógica del spawn
        {
            SelectRandomPrefab();
        }
    }
    public bool CallSpawn()
    {
        if (!IsOnCooldown && IsServer) // Solo el servidor puede llamar al spawn
        {
            SpawnPrefab();
            StartCoroutine(CooldownRoutine());
            return true;
        }
        return false;
    }

    private void SelectRandomPrefab()
    {
        if (prefabsToSpawn.Length > 0)
        {
            int randomIndex = Random.Range(0, prefabsToSpawn.Length);
            selectedPrefab = prefabsToSpawn[randomIndex];
        }
        else
        {
            Debug.LogWarning("No se han asignado prefabs para spawner en " + gameObject.name);
        }
    }

    private void SpawnPrefab()
    {
        if (selectedPrefab != null)
        {
            GameObject instance = Instantiate(selectedPrefab, transform.position, transform.rotation);
            NetworkObject networkObject = instance.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.Spawn();
            }
            else
            {
                Debug.LogError("El prefab seleccionado no tiene un componente NetworkObject: " + selectedPrefab.name);
                Destroy(instance);
            }

            SelectRandomPrefab();
        }
        else
        {
            Debug.LogWarning("No se ha seleccionado un prefab para spawner en " + gameObject.name);
        }
    }

    private IEnumerator CooldownRoutine()
    {
        IsOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        IsOnCooldown = false;
    }
}
