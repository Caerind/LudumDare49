using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtesterZoneComponent : MonoBehaviour
{
    public float radius = 15.0f;
    public int autoSpawnProtesters = 50;

    private void Start()
    {
        transform.position = transform.position.ToVector2().ToVector3();

        SpawnStartingProtesters();
    }

    public Vector2 GetRandomPointInZone()
    {
        return transform.position.ToVector2() + Random.insideUnitCircle * radius;
    }

    public void SpawnStartingProtesters()
    {
        for (int i = 0; i < autoSpawnProtesters; ++i)
        {
            GameManager.Instance.SpawnProtester(GetRandomPointInZone());
        }

    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, radius);
    }
#endif // UNITY_EDITOR
}
