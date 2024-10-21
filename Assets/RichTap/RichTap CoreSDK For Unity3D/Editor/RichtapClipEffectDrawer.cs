using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RichTap.Editor
{
    [CustomPropertyDrawer(typeof(Source.RichtapClipEffect))]
    public class RichtapClipEffectDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = position;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty _clip = property.FindPropertyRelative("clip");
            SerializedProperty _amp = property.FindPropertyRelative("amplitude");
            SerializedProperty _freq = property.FindPropertyRelative("frequency");
            SerializedProperty _loopCount = property.FindPropertyRelative("loopCount");
            SerializedProperty _loopInterval = property.FindPropertyRelative("loopInterval");

            rect.height = EditorGUIUtility.singleLineHeight * 1.1F;

            EditorGUI.PropertyField(rect, _clip);
            rect.y += EditorGUIUtility.singleLineHeight * 1.1F + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(rect, _amp);
            rect.y += EditorGUIUtility.singleLineHeight * 1F + EditorGUIUtility.standardVerticalSpacing;

            rect.x += EditorGUIUtility.labelWidth;
            rect.width -= EditorGUIUtility.labelWidth;
            GUIStyle style = GUI.skin.label;
            TextAnchor defaultAlignment = GUI.skin.label.alignment;
            style.alignment = TextAnchor.UpperLeft;
            EditorGUI.LabelField(rect, "Weak", style);
            style.alignment = TextAnchor.UpperRight;
            EditorGUI.LabelField(rect, "Strong", style);
            GUI.skin.label.alignment = defaultAlignment;
            rect.y += EditorGUIUtility.singleLineHeight * 1.1F + EditorGUIUtility.standardVerticalSpacing;
            rect.x -= EditorGUIUtility.labelWidth;
            rect.width += EditorGUIUtility.labelWidth;

            EditorGUI.PropertyField(rect, _freq);
            rect.y += EditorGUIUtility.singleLineHeight * 1F + EditorGUIUtility.standardVerticalSpacing;

            rect.x += EditorGUIUtility.labelWidth;
            rect.width -= EditorGUIUtility.labelWidth;
            style.alignment = TextAnchor.UpperLeft;
            EditorGUI.LabelField(rect, "Low", style);
            style.alignment = TextAnchor.UpperRight;
            EditorGUI.LabelField(rect, "High", style);
            GUI.skin.label.alignment = defaultAlignment;
            rect.y += EditorGUIUtility.singleLineHeight * 1.1F + EditorGUIUtility.standardVerticalSpacing;
            rect.x -= EditorGUIUtility.labelWidth;
            rect.width += EditorGUIUtility.labelWidth;

            EditorGUI.PropertyField(rect, _loopCount);
            rect.y += EditorGUIUtility.singleLineHeight * 1.1F + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(rect, _loopInterval);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();


        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 8 + EditorGUIUtility.standardVerticalSpacing * 7;
        }
    }
}
