using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name.Contains("Player"))
        {
            sr.color = Color.red;
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Contains("Player"))
        {
            sr.color = Color.white; 
        }
    }
}
