using UnityEngine;

public class JoystickController : MonoBehaviour
{
    [SerializeField] Joystick movementJoystick;
    [SerializeField] float playerSpeed;
    [SerializeField] Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (movementJoystick.Direction.y != 0)
        {
            rb.velocity = new(movementJoystick.Direction.x * playerSpeed, movementJoystick.Direction.y * playerSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}
