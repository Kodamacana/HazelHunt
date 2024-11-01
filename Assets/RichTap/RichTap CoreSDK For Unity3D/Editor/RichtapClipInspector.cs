using RichTap.Common;
using UnityEditor;
using UnityEngine;

namespace RichTap.Editor
{
    [CustomEditor(typeof(RichtapClip))]
    [CanEditMultipleObjects]
    public class RichtapClipInspector : UnityEditor.Editor
    {
        SerializedProperty content;

        public static GUIContent richtapClipLabel = EditorGUIUtility.TrTextContent("Content", "The content of RichtapClip asset.");

        void OnEnable()
        {
            content = serializedObject.FindProperty("content");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            RichtapClip richtapClip = (RichtapClip)target;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea(richtapClip.GetContent());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }
    }
}