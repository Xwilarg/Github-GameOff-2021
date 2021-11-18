using UnityEditor;
using UnityEngine;

namespace Bug.Editor
{
	[CustomPropertyDrawer(typeof(LargeSpriteAttribute))]
	public class LargeSpriteDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * 4;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Sprite), false);
		}
	}
}
