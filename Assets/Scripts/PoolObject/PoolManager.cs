using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<ObjectPool> pools;

    public static PoolManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public PoolableObject GetObjectFromPool(string poolName)
    {
        ObjectPool pool = pools.Find(p => p.objTag.Equals(poolName));
        if (pool != null)
        {
            return pool.GetObject();
        }

        Debug.LogWarning($"Pool with name {poolName} not found!");
        return null;
    }
}
