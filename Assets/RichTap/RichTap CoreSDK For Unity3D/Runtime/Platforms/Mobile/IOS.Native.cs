using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_IOS && !UNITY_EDITOR)
using UnityEngine.iOS;
#endif

namespace RichTap.Platforms.Mobile
{
    public static class IOSSDKBridge
    {

        private const int IOS_MINIMUM_API = 13;
        private static int ApiLevel = 1;

#if UNITY_IOS && !UNITY_EDITOR
        private const string DllName = "__Internal";
		

		[DllImport(DllName)]
		internal static extern bool InvokeIsRichTapEffectSupported();

		[DllImport(DllName)]
		internal static extern void InvokePlayHaptics(string data, int loop, int amplitude, int interval, int frequency);

		[DllImport(DllName)]
		internal static extern void InvokePlayPrebake(int playId, int amplitude);

		[DllImport(DllName)]
		internal static extern void InvokeStop();

		[DllImport(DllName)]
		internal static extern void InvokeSendLoopParameters(int amplidue, int interval, int frequency);
#endif

        public static void Initialize()
        {
#if UNITY_IOS && !UNITY_EDITOR
			string versionString = Device.systemVersion;
			string[] versionArray = versionString.Split('.');
			int.TryParse(versionArray[0], out ApiLevel);
#endif
        }



        public static void Play(string content, int amplitude, int frequency, int loopCount, int loopInterval)
        {
#if UNITY_IOS && !UNITY_EDITOR
			InvokePlayHaptics(
                content, 
                Common.RichtapEffect.LoopCountCheck(loopCount),
                Common.RichtapEffect.AmplitudeCheck(amplitude),
                Common.RichtapEffect.LoopIntervalCheck(loopInterval),
                Common.RichtapEffect.FrequencyCheck(frequency)
                );
#endif
        }

        public static void Play(int preset, int amplitude)
        {
#if UNITY_IOS && !UNITY_EDITOR
            InvokePlayPrebake(preset, Common.RichtapEffect.AmplitudeCheck(amplitude));
#endif
        }

        public static void Stop()
        {
#if UNITY_IOS && !UNITY_EDITOR
			InvokeStop();
#endif
        }

        public static void UpdateParams(int amplitude, int frequency, int loopInterval)
        {
#if UNITY_IOS && !UNITY_EDITOR
			InvokeSendLoopParameters(
                Common.RichtapEffect.AmplitudeCheck(amplitude),
                Common.RichtapEffect.LoopIntervalCheck(loopInterval),
                Common.RichtapEffect.FrequencyCheck(frequency)
            );
#endif
        }
        public static bool IsRichtapEffectSupported()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return IsVibrationEffectSupported() && InvokeIsRichTapEffectSupported();
#endif
            return false;
        }

        [UnityEngine.Scripting.Preserve]
        private static bool IsVibrationEffectSupported() => ApiLevel >= IOS_MINIMUM_API;
    }
}
