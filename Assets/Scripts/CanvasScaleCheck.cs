using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CanvasScalerAdjuster : MonoBehaviour
{
    [Header("Match Width Or Height Values")]
    public float matchFor16to9 = 0.55f;
    public float matchFor4to3 = 1f;
    public float matchFor21to9 = 0.2f;
    public float matchFor18to9 = 0f;

    private CanvasScaler canvasScaler;

    void Update()
    {
        if (!TryGetComponent(out canvasScaler))
        {
            Debug.LogError("CanvasScaler component not found!");
            return;
        }

        AdjustCanvasScale();
    }

    private void AdjustCanvasScale()
    {
        float aspectRatio = (float)Screen.width / Screen.height;

        if (Mathf.Approximately(aspectRatio, 4f / 3f)) // 4:3
        {
            canvasScaler.matchWidthOrHeight = matchFor4to3;
        }
        else if (Mathf.Approximately(aspectRatio, 16f / 9f)) // 16:9
        {
            canvasScaler.matchWidthOrHeight = matchFor16to9;
        }
        else if (Mathf.Approximately(aspectRatio, 21f / 9f)) // 21:9
        {
            canvasScaler.matchWidthOrHeight = matchFor21to9;
        }
        else if (Mathf.Approximately(aspectRatio, 18f / 9f)) // 18:9
        {
            canvasScaler.matchWidthOrHeight = matchFor18to9;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = CalculateClosestMatch(aspectRatio);
            Debug.Log("Undefined aspect ratio. Calculated matchWidthOrHeight: " + canvasScaler.matchWidthOrHeight);
        }
    }

    private float CalculateClosestMatch(float aspectRatio)
    {
        float[] predefinedRatios = { 4f / 3f, 16f / 9f, 21f / 9f, 18f / 9f };
        float[] predefinedMatches = { matchFor4to3, matchFor16to9, matchFor21to9, matchFor18to9 };

        float closestRatio = predefinedRatios[0];
        float closestMatch = predefinedMatches[0];
        float minDifference = Mathf.Abs(aspectRatio - closestRatio);

        for (int i = 1; i < predefinedRatios.Length; i++)
        {
            float difference = Mathf.Abs(aspectRatio - predefinedRatios[i]);
            if (difference < minDifference)
            {
                minDifference = difference;
                closestRatio = predefinedRatios[i];
                closestMatch = predefinedMatches[i];
            }
        }

        return closestMatch;
    }
}
