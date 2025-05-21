using Data;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Setting_Scriptable))]
public class SettingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Setting_Scriptable setting = (Setting_Scriptable)target;
        SerializedProperty rarityArray = serializedObject.FindProperty("Rarity_Percentage");
        
        int index = 0;
        foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
        {           
            SerializedProperty element = rarityArray.GetArrayElementAtIndex(index);

            Color rarityColor = Net_Utils.RarityCircleColor(rarity);
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = rarityColor;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(rarity.ToString(), labelStyle, GUILayout.Width(100));
            EditorGUILayout.PropertyField(element, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            index++;
        }



        float total = 0f;
        foreach (float value in setting.Rarity_Percentage)
        {
            total += value;
        }

        Color displayColor = (Mathf.Approximately(total, 100f)) ? Color.green : Color.red;

        GUIStyle style = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = displayColor }
        };
        EditorGUILayout.LabelField("Total: " + total, style);

        serializedObject.ApplyModifiedProperties();
    }
}
