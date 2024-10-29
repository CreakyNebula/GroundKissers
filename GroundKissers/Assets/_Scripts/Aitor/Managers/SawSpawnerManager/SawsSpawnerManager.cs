using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawsSpawnerManager : MonoBehaviour
{
    [SerializeField] private List<SawsSpawnPoint> spawnPoints; // Lista de puntos de spawn con su prefab específico
    [SerializeField] private float timeBetweenSpawnRounds = 5f;
    [SerializeField] private float minTimeBetweenSpawns = 0.5f;
    [SerializeField] private float maxTimeBetweenSpawns = 2f;
    [SerializeField] private int maxSpawnsPerRound = 3;

    private bool isSpawning = false;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
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

            // Instanciar el prefab asignado al punto de spawn
            Instantiate(selectedPoint.prefabToSpawn, selectedPoint.spawnLocation.position, selectedPoint.spawnLocation.rotation);

            return true;
        }
        else
        {
            Debug.LogWarning("No hay puntos de spawn con un prefab asignado disponible en este momento.");
            return false;
        }
    }

    public bool IsSpawning()
    {
        return isSpawning;
    }
}
