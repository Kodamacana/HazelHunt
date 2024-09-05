using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    List<Rigidbody2D> limbs;
    private PoolManager poolManager;

    private void Start()
    {
        poolManager = PoolManager.Instance;
    }

    public void KilledThePlayer(Vector2 direction, Transform playerPosition)
    {
        limbs = new List<Rigidbody2D>();

        PoolableObject limbsPool = poolManager.GetObjectFromPool("limbs");
        limbsPool.transform.SetPositionAndRotation(playerPosition.position, Quaternion.identity);

        for (int i = 0; i < limbsPool.transform.childCount; i++)
        {
            limbs.Add(limbsPool.transform.GetChild(i).GetComponent<Rigidbody2D>());
        }

        ThrowingTheLimb(direction);
       // StartBleeding();
    }

    private void StartBleeding()
    {

    }

    private void ThrowingTheLimb(Vector2 direction)
    {
        foreach (var limb in limbs)
        {
            limb.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f));
            limb.gameObject.SetActive(true);
            Vector2 newDirection = new Vector2(Random.Range(direction.x - 1.0f, direction.x + 1.0f), Random.Range(direction.y - 1.0f, direction.y + 1.0f));
            limb.linearVelocity = newDirection * Random.Range(1,6f);
        }
    }

    private void FixedUpdate()
    {
        if (limbs == null)
            return;

        if (limbs.Count <= 0)
            return;
        
        for (int i = 0; i < limbs.Count; i++)
        {
            limbs[i].linearVelocity *= (1 - 0.03f);
        }
    }
}
