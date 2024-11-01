using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RichTap.ADB;

namespace RichTap.Editor
{
    public class ADBSettingWizard : ScriptableWizard
    {

        public string adbPath = Environment.GetEnvironmentVariable("ADB_PATH");
        public int hostPort = ADBConnection.localPort;
        private readonly int tartgetPort = ADBConnection.targetPort;

        static ADBSettingWizard wizard;

        [MenuItem("Window/RichTap/ADBSetting")]
        public static void CreateWizard()
        {
            if (wizard != null)
                return;
            wizard = ScriptableWizard.DisplayWizard<ADBSettingWizard>("ADB Setting", "connect");
            wizard = null;
        }

        public bool Init()
        {
            String deviceId = ADBExecutor.GetFirstConnectedAndroidDevice();
            if (deviceId == null)
            {
                EditorUtility.DisplayDialog("error", "Please make sure that your computer has been connected to an Android device that supports the RichTap Haptic function, and the ADB debugging function has been activated.", "ok");
                return false;
            }
            if (deviceId != null && deviceId.Length > 0)
            {
                ADBExecutor.ConnectDevice(deviceId, hostPort, tartgetPort);
            }
            return ADBExecutor.IsForwarding();
        }


        private void OnWizardCreate()
        {
            if (!System.IO.File.Exists(adbPath))
            {
                EditorUtility.DisplayDialog("error", "ADB path is not exists.", "ok");
                CreateWizard();
                return;
            }

            Init();
            Environment.SetEnvironmentVariable("ADB_PATH", adbPath, EnvironmentVariableTarget.User);
            ADBConnection.localPort = hostPort;
            ADBConnection.Instance.Connect();
        }
    }
}
