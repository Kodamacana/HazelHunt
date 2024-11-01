using RichTap.Platforms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RichTap.Internal
{



    internal static class RichtapProvider 
    {
        private readonly static Dictionary<System.Type, object> renderers = new Dictionary<System.Type, object>();

        public static void InitRenderer()
        {
            IEnumerable<System.Reflection.Assembly> enumerable = Reflections.GetCompatibleAssemblies();
            foreach (System.Reflection.Assembly assembly in enumerable)
            {
                IEnumerable<System.Type> types = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IRichtapRenderer)) && !t.IsAbstract);
                foreach (System.Type type in types)
                {
                    Debug.Log(type);
                    object instance = System.Activator.CreateInstance(type);
                    if (instance == null)
                    {
                        return;
                    }
                    if ((bool)type.GetMethod(Reflections.INIT_RENDERER_METHOD_NAME)?.Invoke(instance, null))
                    {
                        renderers.Add(type, instance);
                    }
                }

            }
        }

        
        public static void RenderEffect()
        {
            foreach (var renderer in renderers)
            {
                if ((bool)renderer.Key.GetMethod(Reflections.IS_AVAILABLE_METHOD_NAME)?.Invoke(renderer.Value, null))
                {
                    renderer.Key.GetMethod(Reflections.RENDER_EFFECT_METHOD_NAME)?.Invoke(renderer.Value, null);
                }
            }
        }

        public static void QuitRenderer()
        {
            foreach (var renderer in renderers)
            {
                renderer.Key.GetMethod(Reflections.QUIT_RENDERER_METHOD_NAME)?.Invoke(renderer.Value, null);
            }
        }

        public static bool IsRichtapEffectAvailable()
        {
            foreach (var renderer in renderers)
            {
                return (bool)renderer.Key.GetMethod(Reflections.IS_AVAILABLE_METHOD_NAME)?.Invoke(renderer.Value, null);
            }
            return false;
        }
        
    }
}
