using UnityEngine;

[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class BombDamageSFX : MonoBehaviour
{
    private static BombDamageSFX _instance;

    public static BombDamageSFX Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private float duration = 3f;
    private float startFrequency = 240f;
    private float endFrequency = 5000f;
    private AudioLowPassFilter lowPassFilter;
    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Start()
    {
        lowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    void Update()
    {
        if (isRunning && lowPassFilter != null)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                lowPassFilter.cutoffFrequency = Mathf.Lerp(startFrequency, endFrequency, t);
            }
            else
            {
                lowPassFilter.cutoffFrequency = endFrequency;
                lowPassFilter.enabled = false;
                isRunning = false;
            }
        }
    }

    public void StartLowPassAdjustment()
    {
        if (lowPassFilter != null)
        {
            elapsedTime = 0f;
            isRunning = true;
            lowPassFilter.enabled = true;
            lowPassFilter.cutoffFrequency = startFrequency;
        }
    }

}