using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RichTap.Types;
using RichTap.Common;

namespace RichTap.Core
{
    public partial class REM
    {

        private static REM _instance;
        public static REM Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new REM();
                }
                return _instance;
            }
        }

        private CurrentEffect current;
        private UpdateInfo updateInfo;
        private double timeSinceGameStart;

        public void PlayEffect(RichtapEffect effect)
        {
            current.Play();
            current.Effect(effect);
            timeSinceGameStart = -Time.realtimeSinceStartup;
        }
        public void StopEffect()
        {
            current.Stop();
            timeSinceGameStart = -Time.realtimeSinceStartup;
        }
        public void UpdateEffect(int amplitude, int frequency, int loopInterval)
        {
            current.Update();
            updateInfo.Amplitude(amplitude);
            updateInfo.Frequency(frequency);
            updateInfo.LoopInterval(loopInterval);
            timeSinceGameStart = -Time.realtimeSinceStartup;
        }
        public State GetCurrentEffectState()
        {
            return current.state;
        }
        public CurrentEffect GetCurrentEffect()
        {
            return current;
        }
        public UpdateInfo GetUpdateInfo()
        {
            return updateInfo;
        }
        public double GetVectorTimeSinceGameStart()
        {
            return timeSinceGameStart;
        }

        

    }




}
