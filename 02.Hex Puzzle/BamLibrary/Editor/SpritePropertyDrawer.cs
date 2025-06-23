using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Sprite))]
public class SpritePropertyDrawer : PropertyDrawer
{
	private const float TextureSize = 64;
	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) => 64;
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, prop);
            
		if (prop.hasMultipleDifferentValues)
		{
			EditorGUI.PropertyField(position, prop);
		}
		else
		{
			GUI.Label(position, prop.displayName);
			position.width = EditorGUIUtility.labelWidth;
			position.x += position.width;
			position.width = TextureSize;
			position.height = TextureSize;
			prop.objectReferenceValue = EditorGUI.ObjectField(position, prop.objectReferenceValue, typeof(Sprite), false);
		}

		EditorGUI.EndProperty();
	}
}