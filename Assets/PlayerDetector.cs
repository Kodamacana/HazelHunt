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
            sr.color = new Color(1,0,0, sr.color.a);
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Contains("Player"))
        {
            sr.color = new Color(0.63f, 0.53f, 0.53f, 0.4f);
        }
    }
}
