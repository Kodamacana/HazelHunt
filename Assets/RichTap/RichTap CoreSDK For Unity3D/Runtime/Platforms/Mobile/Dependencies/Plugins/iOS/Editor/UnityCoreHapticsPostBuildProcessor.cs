#if UNITY_EDITOR_OSX
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System;
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;



public static class UnityCoreHapticsPostBuildProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            PBXProject proj = new PBXProject();
            string name = PBXProject.GetPBXProjectPath(buildPath);
            proj.ReadFromFile(name);

            string guid;
#if UNITY_EDITOR_OSX && UNITY_2019_3_OR_NEWER
            guid = proj.GetUnityFrameworkTargetGuid();
#else
            guid = proj.TargetGuidByName("Unity-iPhone");
#endif
            proj.AddFrameworkToProject(guid, "CoreHaptics.framework", true);
            proj.WriteToFile(name);
        }
    }
    
}
#endif

