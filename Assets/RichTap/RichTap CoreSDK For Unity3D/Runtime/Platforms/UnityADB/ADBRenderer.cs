#if UNITY_EDITOR
using RichTap.Common;
using RichTap.Source;
using RichTap.Types;
using UnityEngine;
using RichTap.ADB;

namespace RichTap.Platforms.UnityADB
{
    public sealed class ADBRenderer : GenericAbstractRenderer
    {
        #region Device info
        private const string DEVICE_NAME = "UnityEditor";
        private const string DESCRIPTION = "ADB communication with android";
        private const string VERSION = "1.0";
        #endregion

        public override string DeviceName()
        {
            return DEVICE_NAME;
        }

        public override string Description()
        {
            return DESCRIPTION;
        }

        public override string Version()
        {
            return VERSION;
        }

        public override bool Init()
        {
            
            string deviceId = ADBExecutor.GetFirstConnectedAndroidDevice();
            if (deviceId == null)
            {
                Debug.LogWarning("No connected android device found.");
                return false;
            }
            ADBExecutor.ConnectDevice(deviceId, ADBConnection.localPort, ADBConnection.targetPort);
            ADBConnection.Instance.Connect();
            return true;
            
        }

        public override bool IsRichtapAvailable()
        { 
            return true;
        }

        public override bool Quit()
        {
            return true;
        }


        public override void StopRendering()
        {
            Debug.Log("Stop rendering");
            ADBConnection.Instance.Stop();
            isHapticPlaying = false;
        }

        public override void StartRendering(RichtapEffect effect)
        {
            if (effect is RichtapClipEffect)
            {
                RichtapClip clip = (effect as RichtapClipEffect).GetClip();
                int duration = RichtapUtility.GetClipDuration(clip);
                ADBConnection.Instance.Play(clip.GetContent(), clip.GetClipName(), duration, effect.GetAmplitude(), (effect as RichtapClipEffect).GetFrequency(), (effect as RichtapClipEffect).GetLoopCount(), (effect as RichtapClipEffect).GetLoopInterval());
                isHapticPlaying = true;
                float realTime = Time.realtimeSinceStartup;
                expectedEndTimestamp = realTime + (duration * 1f / 1000);
            }
            else if (effect is RichtapPresetEffect)
            {
                int preset = (int)(effect as RichtapPresetEffect).GetPreset();
                int amplitude = effect.GetAmplitude();
                ADBConnection.Instance.PlayPrebake(preset, amplitude);
                isHapticPlaying = true;
            }
        }

        public override void Update(UpdateInfo updateInfo)
        {
            // DO NOTHING
        }
    }
}
#endif //UNITY_EDITOR
