using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class KillPlayer : MonoBehaviour
{
    List<Rigidbody2D> limbs;
    private PoolManager poolManager;

    private void Start()
    {
        poolManager = PoolManager.Instance;
    }

    public void KilledThePlayer(Vector2 direction, Transform playerPosition, bool isMine)
    {
        limbs = new List<Rigidbody2D>();

        PoolableObject limbsPool = poolManager.GetObjectFromPool("limbs");
        limbsPool.transform.SetPositionAndRotation(playerPosition.position, Quaternion.identity);

        for (int i = 0; i < limbsPool.transform.childCount; i++)
        {
            limbs.Add(limbsPool.transform.GetChild(i).GetComponent<Rigidbody2D>());
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        limbsPool.transform.eulerAngles = new Vector3(0, 0, angle);

        ThrowingTheLimb(direction);

        if (!isMine)
            StartCoroutine(GameController.Instance.IncreaseKillScore());
    }

    private void ThrowingTheLimb(Vector2 direction)
    {
        var randomSpawnedLimbCount = Random.Range(0, limbs.Count-1);
        for (int i = 0;i < limbs.Count- randomSpawnedLimbCount; i++)
        {
            limbs[i].transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f));
            limbs[i].gameObject.SetActive(true);
            Vector2 newDirection = new Vector2(Random.Range(direction.x - 1.0f, direction.x + 1.0f), Random.Range(direction.y - 1.0f, direction.y + 1.0f));
            limbs[i].linearVelocity = newDirection * Random.Range(0.2f, 1.4f);
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
