using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public AudioSource musicSource;

    public AudioClip sfx_Char_WalkOnPlatform;
    public AudioClip sfx_Char_Jump;
    public AudioClip sfx_Char_Fall;
    public AudioClip sfx_Char_CollectCoin;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject audioManagerObject = new GameObject("AudioManager");
                instance = audioManagerObject.AddComponent<AudioManager>();
                DontDestroyOnLoad(audioManagerObject);
            }
            return instance;
        }
    }

}
