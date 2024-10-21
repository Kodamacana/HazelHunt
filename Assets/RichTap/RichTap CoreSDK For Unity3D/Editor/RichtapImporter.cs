/*******************************************************************************
# Copyright @2016 - 2022, Shanghai Ruisheng Kaitai Acoustic Science And Technology Co.,
# Ltd. All Rights Reserved.
********************************************************************************/
using UnityEngine;
using UnityEditor;
using UnityEngine.Bindings;
using RichTap.Common;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#elif UNITY_2018_4_OR_NEWER
using UnityEditor.Experimental.AssetImporters;
#endif

namespace RichTap.Editor
{
#if UNITY_2019_4_OR_NEWER
    [ScriptedImporter(version: 1, ext: "he", AllowCaching = true)]
#else
    [ScriptedImporter(1, "he")]
#endif
    public class RichtapImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var clip = RichtapClip.CreateInstance<RichtapClip>();
            clip.Load(ctx.assetPath);
            ctx.AddObjectToAsset("com.richtap.RichtapClip", clip);
            ctx.SetMainObject(clip);
        }
    }
}