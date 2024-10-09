using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsManager : MonoBehaviour
{
    [SerializeField] private List<SpawnPoint> spawnPoints;
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
        List<SpawnPoint> availablePoints = spawnPoints.FindAll(sp => !sp.IsOnCooldown);
        
        if (availablePoints.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            return availablePoints[randomIndex].CallSpawn();
        }
        else
        {
            Debug.LogWarning("No hay puntos de spawn disponibles en este momento.");
            return false;
        }
    }

    public bool IsSpawning()
    {
        return isSpawning;
    }
}
