
namespace RichTap
{
    [UnityEngine.AddComponentMenu("Richtap/RichtapManager")]
    public class RichtapManager : Internal.Singleton<RichtapManager>
    {
        

        protected override void OnApplicationAwaken()
        {
            Internal.RichtapProvider.InitRenderer();
#if UNITY_ANDROID && !UNITY_EDITOR
            Platforms.Mobile.AndroidSDKBridge.Initialize();
#endif
        }

        private void LateUpdate()
        {
            Internal.RichtapProvider.RenderEffect();
        }

        protected override void OnApplicationQuitting()
        {
            Internal.RichtapProvider.QuitRenderer();
        }

        public static bool IsRichtapEffectAvaialable()
        {
            return Internal.RichtapProvider.IsRichtapEffectAvailable();
        }
    }
}
