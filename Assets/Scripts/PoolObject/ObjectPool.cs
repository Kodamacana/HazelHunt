using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] public string objTag;
    [SerializeField] private PoolableObject prefab;
    [SerializeField] private int initialPoolSize = 10;

    private Queue<PoolableObject> pool = new Queue<PoolableObject>();

    void Start()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            PoolableObject obj = Instantiate(prefab, transform);
            obj.Pool = this;
            obj.Pool.objTag = objTag;
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public PoolableObject GetObject()
    {
        if (pool.Count > 0)
        {
            PoolableObject obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            PoolableObject obj = Instantiate(prefab);
            obj.Pool = this;
            obj.Pool.objTag = objTag;
            return obj;
        }
    }

    public void ReturnObject(PoolableObject obj)
    {
        pool.Enqueue(obj);
    }
}
