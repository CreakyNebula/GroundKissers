using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NWSawsSpawner : NetworkBehaviour
{
    [SerializeField] private List<SawsSpawnPoint> spawnPoints; // Lista de puntos de spawn con su prefab específico
    [SerializeField] private float timeBetweenSpawnRounds = 5f;
    [SerializeField] private float minTimeBetweenSpawns = 0.5f;
    [SerializeField] private float maxTimeBetweenSpawns = 2f;
    [SerializeField] private int maxSpawnsPerRound = 3;

    private bool isSpawning = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer) // Solo el servidor controla la lógica de spawn
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawnRounds);

            isSpawning = true;
            int spawnsThisRound = Random.Range(1, maxSpawnsPerRound + 1);

            for (int i = 0; i < spawnsThisRound; i++)
            {
                if (SpawnAtRandomPoint())
                {
                    yield return new WaitForSeconds(Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns));
                }
            }

            isSpawning = false;
        }
    }

    private bool SpawnAtRandomPoint()
    {
        List<SawsSpawnPoint> availablePoints = spawnPoints.FindAll(sp => sp.prefabToSpawn != null);

        if (availablePoints.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            SawsSpawnPoint selectedPoint = availablePoints[randomIndex];

            // Instanciar el prefab en el servidor
            SpawnSaw(selectedPoint.spawnLocation.position, selectedPoint.spawnLocation.rotation, selectedPoint.prefabToSpawn);

            return true;
        }
        else
        {
            Debug.LogWarning("No hay puntos de spawn con un prefab asignado disponible en este momento.");
            return false;
        }
    }

    private void SpawnSaw(Vector3 position, Quaternion rotation, GameObject prefab)
    {
        if (!IsServer) return; // Solo el servidor puede instanciar

        // Instanciar el objeto como un NetworkObject y sincronizar
        GameObject instance = Instantiate(prefab, position, rotation);
        NetworkObject networkObject = instance.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.Spawn(); // Sincronizar con todos los clientes
        }
        else
        {
            Debug.LogError($"El prefab '{prefab.name}' no tiene un componente NetworkObject.");
        }
    }

    public bool IsSpawning()
    {
        return isSpawning;
    }
}
