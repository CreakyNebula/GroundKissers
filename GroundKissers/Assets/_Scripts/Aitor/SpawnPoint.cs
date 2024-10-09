using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabsToSpawn;
    [SerializeField] private float cooldownTime = 5f; // Tiempo de espera después de spawnear
    [SerializeField] private bool isOnCooldown = false;

    private GameObject selectedPrefab;

    public bool IsOnCooldown { get => isOnCooldown; set => isOnCooldown = value; }

    private void Start()
    {
        SelectRandomPrefab();
    }

    public bool CallSpawn()
    {
        if (!IsOnCooldown)
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
            Instantiate(selectedPrefab, transform.position, transform.rotation);
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