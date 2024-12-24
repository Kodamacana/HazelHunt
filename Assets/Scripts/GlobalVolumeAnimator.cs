using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeAnimator : MonoBehaviour
{
    private static GlobalVolumeAnimator _instance;

    public static GlobalVolumeAnimator Instance { get { return _instance; } }


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

    public Volume volume; // Sahnedeki Volume bileþeni
    public float animationDuration = 1f; // Her bir animasyonun süresi

    private DepthOfField depthOfField;
    private ChromaticAberration chromaticAberration;

    private float dofElapsedTime = 0f;
    private float caElapsedTime = 0f;

    private bool isAnimatingDOF = false;
    private bool isAnimatingCA = false;
    private bool caIncreasing = true;

    void Start()
    {
        // Volume Profile'dan bileþenleri al
        if (volume != null)
        {
            if (volume.profile.TryGet(out depthOfField))
            {
                depthOfField.focalLength.value = 0f; // Depth of Field baþlangýç deðeri
            }
            else
            {
                Debug.LogError("Depth of Field effect not found in the Volume Profile!");
            }

            if (volume.profile.TryGet(out chromaticAberration))
            {
                chromaticAberration.intensity.value = 0f; // Chromatic Aberration baþlangýç deðeri
            }
            else
            {
                Debug.LogError("Chromatic Aberration effect not found in the Volume Profile!");
            }
        }
    }

    void Update()
    {
        if (isAnimatingDOF)
        {
            AnimateDepthOfField();
        }

        if (isAnimatingCA)
        {
            AnimateChromaticAberration();
        }
    }

    public void StartDepthOfFieldAnimation()
    {
        if (depthOfField != null)
        {
            dofElapsedTime = 0f;
            isAnimatingDOF = true;
        }
    }

    public void StartChromaticAberrationAnimation()
    {
        if (chromaticAberration != null)
        {
            caElapsedTime = 0f;
            isAnimatingCA = true;
            caIncreasing = true; // Ýlk olarak 0'dan 1'e çýk
        }
    }

    private void AnimateDepthOfField()
    {
        dofElapsedTime += Time.deltaTime;
        float t = dofElapsedTime / animationDuration;

        depthOfField.focalLength.value = Mathf.Lerp(0f, 300f, t);

        if (t >= 1f)
        {
            isAnimatingDOF = false; // Animasyonu durdur
        }
    }

    private void AnimateChromaticAberration()
    {
        caElapsedTime += Time.deltaTime;
        float t = caElapsedTime / 0.075f;

        if (caIncreasing)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(0.0001f, 0.3f, t);
        }
        else
        {
            chromaticAberration.intensity.value = Mathf.Lerp(0.3f, 0.0001f, t);
        }

        if (t >= 1f)
        {
            caElapsedTime = 0f;

            if (caIncreasing)
            {
                caIncreasing = false;
            }
            else
            {
                isAnimatingCA = false;
            }
        }
    }
}
