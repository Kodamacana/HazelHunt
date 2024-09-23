using Unity.Cinemachine;
using UnityEngine;

public class CameraLensAdjuster : MonoBehaviour
{
    public float lens16x9 = 4.2f;
    public float lens21x9 = 3.89f;
    public float lens4x3= 4.95f;

    private CinemachineCamera cam;

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        AdjustLens();
    }

    void AdjustLens()
    {
        float aspectRatio = (float)Screen.width / (float)Screen.height;

        if (Mathf.Approximately(aspectRatio, 16f / 9f))
        {
            cam.Lens.OrthographicSize = lens16x9;
        }
        else if (Mathf.Approximately(aspectRatio, 21f / 9f))
        {
            cam.Lens.OrthographicSize = lens21x9;
        }
        else if (Mathf.Approximately(aspectRatio, 4f/3f))
        {
            cam.Lens.OrthographicSize = lens4x3;
        }
        else
        {
            float interpolatedLens = Mathf.Lerp(lens21x9, lens16x9, Mathf.InverseLerp(21f / 9f, 16f / 9f, aspectRatio));
            cam.Lens.OrthographicSize = interpolatedLens;
        }               
    }

    
}
