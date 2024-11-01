using RichTap.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RichTap.Source
{
    [System.Serializable]
    public class RichtapPresetEffect : RichtapEffect
    {

        [SerializeField] private RichtapPreset preset = RichtapPreset.RT_CLICK;

        public override void Play()
        {
            Core.REM.Instance.PlayEffect(this);
        }

        public RichtapPreset GetPreset()
        {
            return preset;
        }


        public static RichtapPresetEffect BuildEffect(RichtapPreset preset, int amplitude = DEFAULT_AMPLITUDE)
        {
            RichtapPresetEffect effect = new RichtapPresetEffect();
            effect.preset = preset;
            effect.amplitude = amplitude;
            return effect;
        }

        public override string ToString()
        {
            return $"Effect: [{preset}], amplitude: [{amplitude}]";
        }
    }
}
