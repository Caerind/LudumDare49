using System.Collections.Generic;
using UnityEngine;

public class EntityPool
{
    [System.Serializable]
    public class Settings
    {
        public GameObject prefab;
        public int poolSize;
    }

    private Settings settings;
    private List<GameObject> pool;
    private List<GameObject> outOfPoolObjects;

    public EntityPool(Settings settings)
    {
        this.settings = settings;

        pool = new List<GameObject>();
        for (int i = 0; i < settings.poolSize; i++)
        {
            GameObject obj = GameObject.Instantiate(settings.prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject Instantiate()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject createdObject = GameObject.Instantiate(settings.prefab);
        outOfPoolObjects.Add(createdObject);
        return createdObject;
    }

    public GameObject Instantiate(Vector3 position, Quaternion quaternion)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = quaternion;
                return obj;
            }
        }

        GameObject createdObject = GameObject.Instantiate(settings.prefab, position, quaternion);
        outOfPoolObjects.Add(createdObject);
        return createdObject;
    }

    public void Destroy(GameObject obj)
    {
        bool isInPool = pool.Contains(obj);
        if (isInPool)
        {
            obj.SetActive(false);
        }
        else if (outOfPoolObjects.Remove(obj))
        {
            GameObject.Destroy(obj);
        }
        else
        {
            Debug.LogWarning("Destroying an object not belonging to this pool");
            GameObject.Destroy(obj);
        }
    }

    public void DestroyAll()
    {
        foreach (GameObject obj in pool)
        {
            if (obj.activeInHierarchy)
            {
                obj.SetActive(false);
            }
        }
        foreach (GameObject obj in outOfPoolObjects)
        {
            GameObject.Destroy(obj);
        }
        outOfPoolObjects.Clear();
    }
}
