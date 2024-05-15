using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuts : MonoBehaviour
{
    [SerializeField] float forceValue = 75f;
    [SerializeField] float damping = 0.1f; // Azalma katsayýsý

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(PositionNut());
    }

    private IEnumerator PositionNut()
    {
        while (transform.parent == null)
        {
            yield return null;
        }
        transform.localPosition = new Vector3(0.52f, 0, 0);
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = false;
    }

    private void StartMovement(Vector2 direction)
    {
        rb.velocity = direction *forceValue;
    }

    public void FireNut(Vector2 originalDirection)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        transform.SetParent(null);
        StartMovement(originalDirection);
    }

    private void FixedUpdate()
    {
        // Hýzý azalt
        rb.velocity *= (1 - damping);
    }
}
