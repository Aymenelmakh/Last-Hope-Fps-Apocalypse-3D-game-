using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaseSystem : MonoBehaviour
{
    public GameObject[] zombiePrefabs;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 10f;
    [SerializeField] private float waveTimer = 0f;
    private int CurrentwaveNumber = 1;
    public int zombiePerWave = 5;
    public static int nb_of_zombies = 0;

    void Update()
    {
        LoadSetting();
        if (CurrentwaveNumber == 10)
        {
            return;
        }
        waveTimer += Time.deltaTime;
        int nb = Mathf.RoundToInt(waveTimer);
        if (waveTimer >= timeBetweenWaves)
        {
            CreateWave();
        }
    }

    // void CreateWave()
    // {
    //     waveTimer = 0f;
    //     zombiePerWave += 2; // make the wave getting hard after each wave
    //     float minDistance = 4f; // gap between zombies spawned
    //     for (int i = 0; i < zombiePerWave; i++)
    //     {
    //         int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
    //         Transform spwanPoint = spawnPoints[randomSpawnIndex];
    //         GameObject randomZombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Length)];
    //         Vector3 spawnPosition = spwanPoint.position + Random.insideUnitSphere * minDistance;
    //         spawnPosition.y = spwanPoint.position.y;
    //         Instantiate(randomZombiePrefab, spawnPosition, spwanPoint.rotation);
    //     }
    //     CurrentwaveNumber++;
    // }
    void CreateWave()
    {
        waveTimer = 0f;
        StartCoroutine(SpawnWaveOverTime());
        nb_of_zombies += zombiePerWave;
        CurrentwaveNumber++;
    }

    IEnumerator SpawnWaveOverTime()
    {
        float minDistance = 4f;
        for (int i = 0; i < zombiePerWave; i++)
        {
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomSpawnIndex];
            GameObject randomZombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Length)];
            Vector3 spawnPosition = spawnPoint.position + Random.insideUnitSphere * minDistance;
            spawnPosition.y = spawnPoint.position.y;
            Instantiate(randomZombiePrefab, spawnPosition, spawnPoint.rotation);

            yield return new WaitForSeconds(0.5f); // stagger each spawn
        }
    }
    void LoadSetting()
    {
        if (PlayerPrefs.HasKey("TimeBetweenWaves"))
        {
            timeBetweenWaves = PlayerPrefs.GetFloat("TimeBetweenWaves");
        }
        if (PlayerPrefs.HasKey("ZombiesPerWave"))
        {
            zombiePerWave = PlayerPrefs.GetInt("ZombiesPerWave");
        }
    }
}
