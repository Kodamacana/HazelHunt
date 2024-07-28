using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    public ObjectPool Pool { get; set; }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        Pool.ReturnObject(this);
    }
}
