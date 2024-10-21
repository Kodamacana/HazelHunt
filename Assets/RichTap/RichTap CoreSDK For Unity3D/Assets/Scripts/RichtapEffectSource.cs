using RichTap.Source;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sample use of RichtapEffect. Attach this script to any game object which triggers a vibration, adjust properties in the inspector.
/// Also you can define a specified HE in the inspector and play it directly.
/// </summary>
public class RichtapEffectSource : MonoBehaviour
{


    [SerializeField] private bool isBuiltInEffect = false;

    private RichTap.Common.RichtapEffect effect;
    private RichTap.Common.RichtapClip[] clips;
    private RichTap.Common.RichtapPreset[] presets = new RichTap.Common.RichtapPreset[]
    {
        RichTap.Common.RichtapPreset.RT_CLICK,
        RichTap.Common.RichtapPreset.RT_DOUBLE_CLICK,
        RichTap.Common.RichtapPreset.RT_SOFT_CLICK,
        RichTap.Common.RichtapPreset.RT_TICK,
        RichTap.Common.RichtapPreset.RT_THUD,
        RichTap.Common.RichtapPreset.RT_FAILURE,
        RichTap.Common.RichtapPreset.RT_SUCCESS,
        RichTap.Common.RichtapPreset.RT_RAMP_UP,
        RichTap.Common.RichtapPreset.RT_TOGGLE_SWITCH,
        RichTap.Common.RichtapPreset.RT_LONG_PRESS,
        RichTap.Common.RichtapPreset.RT_VIRTUAL_KEY,
        RichTap.Common.RichtapPreset.RT_KEYBOARD_TAP,
        RichTap.Common.RichtapPreset.RT_CLOCK_TICK,
        RichTap.Common.RichtapPreset.RT_CALENDAR_DATE,
        RichTap.Common.RichtapPreset.RT_CONTEXT_CLICK,
        RichTap.Common.RichtapPreset.RT_KEYBOARD_RELEASE,
        RichTap.Common.RichtapPreset.RT_VIRTUAL_KEY_RELEASE,
        RichTap.Common.RichtapPreset.RT_TEXT_HANDLE_MOVE,
        RichTap.Common.RichtapPreset.RT_ENTRY_BUMP,
        RichTap.Common.RichtapPreset.RT_DRAG_CROSSING,
        RichTap.Common.RichtapPreset.RT_GESTURE,
        RichTap.Common.RichtapPreset.RT_CONFIRM,
        RichTap.Common.RichtapPreset.RT_REJECT,
        RichTap.Common.RichtapPreset.RT_BOMB,
        RichTap.Common.RichtapPreset.RT_SWORD,
        RichTap.Common.RichtapPreset.RT_GUNSHOT,
        RichTap.Common.RichtapPreset.RT_SPEED_UP,
        RichTap.Common.RichtapPreset.RT_JUMP,
        RichTap.Common.RichtapPreset.RT_DRUM,
        RichTap.Common.RichtapPreset.RT_COIN_DROP,
        RichTap.Common.RichtapPreset.RT_HEARTBEAT,
        RichTap.Common.RichtapPreset.RT_PLUCKING,
        RichTap.Common.RichtapPreset.RT_DRAWING_ARROW,
        RichTap.Common.RichtapPreset.RT_CAMERA_SHUTTER,
        RichTap.Common.RichtapPreset.RT_FIREWORKS,
        RichTap.Common.RichtapPreset.RT_SNIPER_RIFLE,
        RichTap.Common.RichtapPreset.RT_ASSAULT_RIFLE,
        RichTap.Common.RichtapPreset.RT_CYMBAL,
        RichTap.Common.RichtapPreset.RT_TAMBOURINE,
        RichTap.Common.RichtapPreset.RT_FAST_MOVING,
        RichTap.Common.RichtapPreset.RT_FLY,
        RichTap.Common.RichtapPreset.RT_FOOTSTEP,
        RichTap.Common.RichtapPreset.RT_ICE,
        RichTap.Common.RichtapPreset.RT_LIGHTNING,
        RichTap.Common.RichtapPreset.RT_SPRING,
        RichTap.Common.RichtapPreset.RT_SWING,
        RichTap.Common.RichtapPreset.RT_WIND,
        RichTap.Common.RichtapPreset.RT_VICTORY,
        RichTap.Common.RichtapPreset.RT_AWARD,
        RichTap.Common.RichtapPreset.RT_GAMEOVER,
    };


    public EventHandler<RichtapEffectClipArgs> OnResourcesLoaded;
    public class RichtapEffectClipArgs : EventArgs
    {
        public string[] options;
    }

    private void Start()
    {
        string[] names;
        if (isBuiltInEffect)
        {
            names = LoadPresets();
        }
        else
        {
            names = LoadClips();
        }
        OnResourcesLoaded?.Invoke(this, new RichtapEffectClipArgs
        {
            options = names
        });
        SelectEffect(0, -1);
    }

    public void Play()
    {
        if (effect != null)
        {
            Debug.Log(effect);
            effect.Play();
        }
    }

    public void Stop()
    {
        if (effect != null)
        {
            effect.Stop();
        }
    }

    public void UpdateParams(int value)
    {
        //RichTap.Core.REM.Instance.UpdateEffect(value, effect.GetFrequency(), effect.GetLoopInterval());
    }

    public void SelectEffect(int index, int loopCount)
    {
        if (isBuiltInEffect)
        {
            effect = RichtapPresetEffect.BuildEffect(presets[index]);
        }
        else
        {
            if (clips != null && clips.Length > 0 && index < clips.Length)
            {
                RichTap.Common.RichtapClip clip = clips[index];
                effect = RichtapClipEffect.BuildEffect(clip, loopCount: loopCount);
            }
        }
        Debug.Log("---->>>>Selected effect: " + effect + " at index: " + index + "<<<<----");
    }

    private string[] LoadClips()
    {
        clips = Resources.LoadAll<RichTap.Common.RichtapClip>("HE");
        string[] names = new string[clips.Length];
        for (int i = 0; i < clips.Length; i++)
        {
            names[i] = clips[i].GetClipName();
        }
        return names;
    }
    private string[] LoadPresets()
    {
        string[] names = new string[presets.Length];
        for (int i = 0; i < presets.Length; i++)
        {
            names[i] = presets[i].ToString();
        }
        return names;
    }

}
