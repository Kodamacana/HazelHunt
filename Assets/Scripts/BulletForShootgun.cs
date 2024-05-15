using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletForShootgun : MonoBehaviour
{

    public void Start()
    {
        Destroy(gameObject, .15f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name.Contains("Player"))
        {
            collision.gameObject.GetComponent<PlayerBase>().OnDamage();
        }
        Destroy(gameObject);
    }
       

    public void InitializeBullet(Vector2 originalDirection, float lag, float angle)
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        originalDirection *= lag;
        rigidbody.velocity = originalDirection *60f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
