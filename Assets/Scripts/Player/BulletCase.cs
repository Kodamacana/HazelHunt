using System.Collections;
using UnityEngine;

public class BulletCase : MonoBehaviour
{
    private Transform ejectionPoint; // Kovanýn fýrlatýlacaðý pozisyon
    private float ejectionForce = 2f;
    private float rotationForce = 300f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        EjectCasing();
    }

    void EjectCasing()
    {
        // Fýrlatma kuvveti uygula
        Vector2 ejectionDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
        rb.AddForce(ejectionDirection * ejectionForce, ForceMode2D.Impulse);

        // Rastgele dönme kuvveti uygula
        rb.AddTorque(Random.Range(-rotationForce, rotationForce));
        StartCoroutine(CloseSimulation());
    }

    IEnumerator CloseSimulation()
    {
        yield return new WaitForSecondsRealtime(1.2f);
        rb.simulated = false;
    }
}
