namespace RichTap.Platforms.Mobile
{
    public class AndroidSDKBridge
    {
        private const string CLASS_PATH = "com.apprichtap.haptic.RichTapUtils";
        private const string METHOD_GETINSTANCE = "getInstance";
        private const string METHOD_INIT = "init";
        private const string METHOD_QUIT = "quit";
        private const string METHOD_STOP = "stop";
        private const string METHOD_PLAY = "playHaptic";
        private const string METHOD_PLAY_PREBAKE = "playExtPrebaked";
        private const string METHOD_UPDATE_PARAMS = "sendLoopParameter";
        private const string METHOD_SUPPORT_RICHTAP = "isSupportedRichTap";


        private const int ANDROID_OREO = 26;
        private static int ApiLevel = 1;
        private static bool isInitialized = false;
        private static bool isRichtapCore = false;

        private static UnityEngine.AndroidJavaObject richtapInstance;


        public static void Initialize()
        {
            if (isInitialized || UnityEngine.Application.platform != UnityEngine.RuntimePlatform.Android)
            {
                return;
            }
#if UNITY_ANDROID
            if (UnityEngine.Application.isConsolePlatform)
            {
                UnityEngine.Handheld.Vibrate();
            }
#endif
            using (UnityEngine.AndroidJavaClass androidSDKVersion = new UnityEngine.AndroidJavaClass("android.os.Build$VERSION"))
            {
                ApiLevel = androidSDKVersion.GetStatic<int>("SDK_INT");
            }
            using UnityEngine.AndroidJavaClass unityPlayer = new UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using UnityEngine.AndroidJavaObject currentActivity = unityPlayer.GetStatic<UnityEngine.AndroidJavaObject>("currentActivity");
            using UnityEngine.AndroidJavaClass richtapUtils = new UnityEngine.AndroidJavaClass(CLASS_PATH);
            richtapInstance = richtapUtils.CallStatic<UnityEngine.AndroidJavaObject>(METHOD_GETINSTANCE);
            if (richtapInstance != null)
            {
                richtapInstance.Call<UnityEngine.AndroidJavaObject>(METHOD_INIT, currentActivity);
                isRichtapCore = richtapInstance.Call<bool>(METHOD_SUPPORT_RICHTAP);
                isInitialized = true;
            }
        }

        public static void Play(string content, int amplitude, int frequency, int loopCount, int loopInterval)
        {
            if (isInitialized && richtapInstance != null)
            {
                richtapInstance.Call(
                    METHOD_PLAY,
                    content,
                    Common.RichtapEffect.LoopCountCheck(loopCount),
                    Common.RichtapEffect.LoopIntervalCheck(loopInterval),
                    Common.RichtapEffect.AmplitudeCheck(amplitude),
                    Common.RichtapEffect.FrequencyCheck(frequency)
                    );
            }
        }

        public static void Play(int preset, int amplitude)
        {
            if (isInitialized && richtapInstance != null)
            {
                richtapInstance.Call(
                    METHOD_PLAY_PREBAKE,
                    preset,
                    Common.RichtapEffect.AmplitudeCheck(amplitude)
                    );
            }
        }

        public static void UpdateParams(int amplitude, int frequency, int loopInterval)
        {
            if (isInitialized && richtapInstance != null)
            {
                richtapInstance.Call(
                    METHOD_UPDATE_PARAMS,
                    Common.RichtapEffect.AmplitudeCheck(amplitude),
                    Common.RichtapEffect.LoopIntervalCheck(loopInterval),
                    Common.RichtapEffect.FrequencyCheck(frequency)
                    );
            }
        }

        public static void Stop()
        {
            if (isInitialized && richtapInstance != null)
            {
                richtapInstance.Call(METHOD_STOP);
            }
        }

        public static void Quit()
        {
            isInitialized = false;
            if (richtapInstance != null)
            {
                richtapInstance.Call(METHOD_QUIT);
            }
        }

        public static bool IsRichtapEffectSupported()
        {
            return IsVibrationEffectSupported() && isRichtapCore;
        }

        private static bool IsVibrationEffectSupported() => ApiLevel >= ANDROID_OREO;

    }
}
