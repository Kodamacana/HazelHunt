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
            rb.linearVelocity = new(movementJoystick.Direction.x * playerSpeed, movementJoystick.Direction.y * playerSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
