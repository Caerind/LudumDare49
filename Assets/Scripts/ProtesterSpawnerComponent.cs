using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtesterSpawnerComponent : MonoBehaviour
{
    public List<Transform> spawnPoints;

    public int totalCount = 100;
    private int remainingCount = 0;
    public float defaultDelay = 2.0f;
    public float randomDelay = 2.0f;
    private float remainingDelay = 0.0f;

    public int GetRemainingCount()
    {
        return remainingCount;
    }

    private void Start()
    {
        remainingCount = totalCount;
        transform.position = transform.position.ToVector2().ToVector3(); // Ensures to be at z=0
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        if (remainingCount > 0)
        {
            remainingDelay -= Time.deltaTime;
            if (remainingDelay < 0.0f)
            {
                remainingCount--;
                remainingDelay += defaultDelay + Random.Range(0.0f, randomDelay);

                // Spawn from a random point
                int spawnPointIndex = Random.Range(0, spawnPoints.Count - 1);
                GameManager.Instance.SpawnProtester(spawnPoints[spawnPointIndex].position.ToVector2());
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(148.0f/255.0f, 0.0f, 211/255.0f);
        foreach (Transform point in spawnPoints)
        {
            Gizmos.DrawSphere(point.position, 1.0f);
        }
    }
}
