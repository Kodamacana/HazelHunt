using TMPro;
using UnityEngine;

public class FpsCalculator : MonoBehaviour
{
    [SerializeField][Range(0f, 1f)] private float _expSmoothingFactor = 0.9f;
    [SerializeField] private float _refreshFrequency = 0.4f;

    private float _timeSinceUpdate = 0f;
    private float _averageFps = 1f;

    [SerializeField] private TMP_Text _text;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        _averageFps = _expSmoothingFactor * _averageFps + (1f - _expSmoothingFactor) * 1f / Time.unscaledDeltaTime;

        if (_timeSinceUpdate < _refreshFrequency)
        {
            _timeSinceUpdate += Time.deltaTime;
            return;
        }

        int fps = Mathf.RoundToInt(_averageFps);
        _text.text = fps.ToString();

        _timeSinceUpdate = 0f;
    }
}
