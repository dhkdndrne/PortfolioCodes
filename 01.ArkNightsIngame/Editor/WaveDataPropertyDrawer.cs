using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bam.Extensions;

[CustomPropertyDrawer(typeof(WaveData))]
public class WaveDataPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float padding = 2f;

        // enemyID, order, interval 필드 영역
        Rect enemyIdRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect orderRect = new Rect(position.x, enemyIdRect.y + lineHeight + padding, position.width, lineHeight);
        Rect intervalRect = new Rect(position.x, orderRect.y + lineHeight + padding, position.width, lineHeight);
        Rect waitForKillRect = new Rect(position.x, intervalRect.y + lineHeight + padding, position.width, lineHeight);
        
        // 각 필드 그리기
        EditorGUI.PropertyField(enemyIdRect, property.FindPropertyRelative("enemyID"), new GUIContent("Enemy ID"));
        EditorGUI.PropertyField(orderRect, property.FindPropertyRelative("order"), new GUIContent("Order"));
        EditorGUI.PropertyField(intervalRect, property.FindPropertyRelative("interval"), new GUIContent("Interval"));
        //EditorGUI.PropertyField(waitForKillRect, property.FindPropertyRelative("waitForKill"), new GUIContent("WaitForKill"));
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float padding = 2f;
        
        return (lineHeight * 4) + (padding * 4);
    }
}
