using UnityEngine;

namespace RichTap.Common
{
    [System.Serializable]
    public abstract class RichtapEffect
    {
        public const int MAX_AMPLITUDE = 511;
        public const int MIN_AMPLITUDE = 0;
        public const int DEFAULT_AMPLITUDE = 255;
        public const int MAX_FREQUENCY = 100;
        public const int MIN_FREQUENCY = -100;
        public const int MIN_LOOP_COUNT = -1;
        public const int MIN_LOOP_INTERVAL = 1;
        public const int DEFAULT_FREQUENCY = 0;


        [SerializeField]
        [Range(MIN_AMPLITUDE, MAX_AMPLITUDE)]
        protected int amplitude = DEFAULT_AMPLITUDE;


        public virtual void Play() { }

        public void Stop()
        {
            Core.REM.Instance.StopEffect();
        }

        public int GetAmplitude() => amplitude;


        public static int AmplitudeCheck(int amplitude)
        {
            if (amplitude >= MIN_AMPLITUDE && amplitude <= MAX_AMPLITUDE)
            {
                return amplitude;
            }
            else
            {
                return DEFAULT_AMPLITUDE;
            }
        }

        public static int FrequencyCheck(int frequency)
        {
            if (frequency >= MIN_FREQUENCY && frequency <= MAX_FREQUENCY)
            {
                return frequency;
            }
            else
            {
                return MAX_FREQUENCY;
            }
        }

        public static int LoopCountCheck(int loopCount)
        {
            if (loopCount >= MIN_LOOP_COUNT)
            {
                return loopCount;
            }
            else
            {
                return MIN_LOOP_COUNT;
            }
        }

        public static int LoopIntervalCheck(int loopInterval)
        {
            if (loopInterval >= MIN_LOOP_INTERVAL)
            {
                return loopInterval;
            }
            else
            {
                return MIN_LOOP_INTERVAL;
            }
        }

    }
}
