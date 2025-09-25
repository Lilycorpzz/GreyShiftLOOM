using UnityEngine;

using System.Collections;

public class LoomSpawner : MonoBehaviour
{
    [Header("Spawning")]
    public Transform[] spawnPoints; // assign SpawnPoint_A and SpawnPoint_B
    public GameObject zombiePrefab; // assign ZombieWorker_Prefab
    public int maxZombies = 6;
    public float spawnInterval = 6f;
    public int spawnPerWave = 1;

    private int liveCount = 0;

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogError("LoomSpawner: Assign spawn points.");
        if (zombiePrefab == null)
            Debug.LogError("LoomSpawner: Assign zombiePrefab.");

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // spawn only if under max
            if (liveCount < maxZombies)
            {
                for (int i = 0; i < spawnPerWave; i++)
                {
                    Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    GameObject z = Instantiate(zombiePrefab, sp.position, Quaternion.identity);
                    ZombieWorkerAI ai = z.GetComponent<ZombieWorkerAI>();
                    if (ai != null)
                    {
                        ai.Initialize(sp); // give the spawn center to the zombie
                        liveCount++;
                        ai.OnDestroyed += () => liveCount--; // decrement when destroyed
                    }
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
