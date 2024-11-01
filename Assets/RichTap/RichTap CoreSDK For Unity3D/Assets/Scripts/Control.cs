using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private Button stop;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Toggle toggle;
    [SerializeField] private RichtapEffectSource source;

    [SerializeField] private Button sceneSwitcher;
    [SerializeField] private Loader.Scene target;


   

    private int amp = 255;
    private int loopCount = -1;
    private int index = 0;

    private void Awake()
    {
        source.OnResourcesLoaded += OnResourcesLoad;
    }

    void Start()
    {
        play.onClick.AddListener(() => 
        {
            source.Play();                   
        });

        stop.onClick.AddListener(() => 
        {
            // All vibration will be stopped, no matter source.Stop() or REM.Instance.StopEffect() is called.
            //RichTap.Core.REM.Instance.StopEffect();
            source.Stop();
        });

        sceneSwitcher.onClick.AddListener(() => 
        {
            Loader.Load(target);
        });

        slider.onValueChanged.AddListener((float value) =>
        {
            amp = (int)(511 * value);
        });

        if (slider.TryGetComponent<EventTrigger>(out EventTrigger trigger))
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(OnSliderValuedDragEnd);
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }
        
        dropdown.onValueChanged.AddListener((int index) => 
        {
            this.index = index;
            source.SelectEffect(index, loopCount);
        });

        toggle.onValueChanged.AddListener((bool check) => 
        {
            if (check)
            {
                loopCount = -1;
            }
            else
            {
                loopCount = 0;
            }
            source.SelectEffect(index, loopCount);
        });
    }

    private void OnResourcesLoad(object sender, RichtapEffectSource.RichtapEffectClipArgs e)
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (string _option in e.options)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
            {
                text = _option
            };
            options.Add(option);
        }
        dropdown.AddOptions(options);
        Debug.Log("Option's count: " + options.Count);
    }

    void OnSliderValuedDragEnd(BaseEventData data)
    {
        source.UpdateParams(amp);
    }

}
