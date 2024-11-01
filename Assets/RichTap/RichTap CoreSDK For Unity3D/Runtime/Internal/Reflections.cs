using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RichTap.Internal
{
    public static class Reflections
    {

        #region ASSEMBLY NAMES
        public const string DEFAULT_ASSEMBLY_NAME = "Assembly-CSharp";
        public const string ASSEMBLY_PREFIX_NAME_FOR_RENDERERS = "RichTap";
        #endregion


        #region METHODS NAMES
        // INFORMATION
        public const string DESCRIPTION_RENDERER_METHOD_NAME = "Description";
        public const string DEVICE_NAME_RENDERER_METHOD_NAME = "DeviceName";
        public const string VERSION_RENDERER_METHOD_NAME = "Version";

        // SETUP
        public const string INIT_RENDERER_METHOD_NAME = "Init";
        public const string QUIT_RENDERER_METHOD_NAME = "Quit";

        // RENDERING
        public const string IS_AVAILABLE_METHOD_NAME = "IsRichtapAvailable";
        public const string RENDER_EFFECT_METHOD_NAME = "RenderRichtapEffect";
        #endregion


        public static IEnumerable<Assembly> GetRendererAssemblies()
        {
            return GetAssemblies(assembly =>
                assembly.FullName.StartsWith(ASSEMBLY_PREFIX_NAME_FOR_RENDERERS)
            );
        }

        public static IEnumerable<Assembly> GetCompatibleAssemblies()
        {
            return GetAssemblies(assembly =>
                assembly.FullName.StartsWith(ASSEMBLY_PREFIX_NAME_FOR_RENDERERS) || 
                assembly.GetName().Name == DEFAULT_ASSEMBLY_NAME ||
                assembly == Assembly.GetExecutingAssembly()
            );
        }

        private static IEnumerable<Assembly> GetAssemblies(Func<Assembly, bool> checker)
        {
            return System.AppDomain.CurrentDomain.GetAssemblies().Where(checker);
        }

    }
}
