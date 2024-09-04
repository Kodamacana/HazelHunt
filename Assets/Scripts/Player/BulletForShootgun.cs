using UnityEngine;

public class BulletForShootgun : MonoBehaviour
{    
    public void InitializeBullet(Vector2 originalDirection, Vector2 velocity)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
               
        originalDirection = new(originalDirection.x * 2f, originalDirection.y * 2f);
        rb.linearVelocity = originalDirection;
    }
}
