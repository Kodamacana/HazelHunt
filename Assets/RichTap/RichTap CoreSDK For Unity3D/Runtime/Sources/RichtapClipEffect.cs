using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RichTap.Common;

namespace RichTap.Source
{
    [System.Serializable]
    public class RichtapClipEffect : RichtapEffect
    {



        [SerializeField]
        [Range(MIN_FREQUENCY, MAX_FREQUENCY)]
        private int frequency = DEFAULT_FREQUENCY;

        [Tooltip("The clip will play only once if set to 0, -1 means infinity loop; otherwise, if the value is greater than 0, the actual playing count will plus 1.")]
        [SerializeField]
        [Min(MIN_LOOP_COUNT)]
        private int loopCount = MIN_LOOP_COUNT;

        [SerializeField]
        [Min(MIN_LOOP_INTERVAL)]
        private int loopInterval = MIN_LOOP_INTERVAL;

        [SerializeField] private RichtapClip clip;


        public override void Play()
        {
            if (clip != null)
            {
                Core.REM.Instance.PlayEffect(this);
            }
        }


        public RichtapClip GetClip()
        {
            return clip;
        }

        public int GetFrequency() => frequency;
        public int GetLoopCount() => loopCount;
        public int GetLoopInterval() => loopInterval;

        public override string ToString()
        {
            if (clip == null)
            {
                return "No clip data.";
            }
            return $"Effect:[{clip.GetClipName()}], amplitude: [{amplitude}], frequency: [{frequency}].";
        }
        public static RichtapClipEffect BuildEffect(RichtapClip clip, int amplitude = DEFAULT_AMPLITUDE, int frequency = DEFAULT_FREQUENCY, int loopCount = MIN_LOOP_COUNT, int loopInterval = MIN_LOOP_INTERVAL)
        {
            RichtapClipEffect effect = new RichtapClipEffect();
            effect.clip = clip;
            effect.amplitude = amplitude;
            effect.frequency = frequency;
            effect.loopCount = loopCount;
            effect.loopInterval = loopInterval;
            return effect;
        }

    }
}
