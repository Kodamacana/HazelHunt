using UnityEngine;

public class BulletForShootgun : MonoBehaviour
{
    public void Start()
    {
        Destroy(gameObject, .25f);
    }
    
    public void InitializeBullet(Vector2 originalDirection, float lag, float angle, Vector2 pdir, Vector2 playerVelocity)
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        originalDirection *= (lag+1.1f);
        
        rigidbody.velocity = (originalDirection +pdir) * (130f + playerVelocity.magnitude);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
