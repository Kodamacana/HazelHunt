using RichTap.Common;
using RichTap.Source;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RichTap.Platforms
{
    public abstract class GenericAbstractRenderer : IRichtapRenderer
    {

        protected double lastBufferStartingTime = 0;
        protected bool isHapticPlaying = false;
        protected float? expectedEndTimestamp = -1;

        public abstract string Description();
        public abstract string DeviceName();
        public abstract string Version();
        public abstract bool Init();
        public abstract bool Quit();
        public abstract bool IsRichtapAvailable();
        public virtual void RenderRichtapEffect()
        {
            double startingTime = Core.REM.Instance.GetVectorTimeSinceGameStart();
            if (startingTime != lastBufferStartingTime)
            {
                lastBufferStartingTime = startingTime;
                Types.State state = Core.REM.Instance.GetCurrentEffectState();
                switch (state)
                {
                    default:
                    case Types.State.Idle:
                        Idle();
                        break;
                    case Types.State.Play:
                        StartRendering(Core.REM.Instance.GetCurrentEffect().effect);
                        break;
                    case Types.State.Update:
                        if (isHapticPlaying)
                        {
                            Update(Core.REM.Instance.GetUpdateInfo());
                        }
                        break;
                    case Types.State.Stop:
                        StopRendering();
                        break;
                }
            }
            else
            {
                if (isHapticPlaying && expectedEndTimestamp != -1 && Time.realtimeSinceStartup > expectedEndTimestamp)
                {
                    StopRendering();
                }
            }
        }


        public virtual void Idle() { }
        public abstract void StartRendering(RichtapEffect effect);
        public abstract void Update(Types.UpdateInfo updateInfo);
        public abstract void StopRendering();
    }
}
