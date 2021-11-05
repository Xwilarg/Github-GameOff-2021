using Bug.Navigation;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;

namespace Bug.Editor.Navigation
{
	[CustomPropertyDrawer(typeof(AreaMaskAttribute))]
	public class AreaMaskAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			int areaIndex = -1;
			string[] areaNames = GameObjectUtility.GetNavMeshAreaNames();

			for (int i = 0; i < areaNames.Length; i++)
			{
				int areaValue = GameObjectUtility.GetNavMeshAreaFromName(areaNames[i]);
				if (areaValue == property.intValue)
					areaIndex = i;
			}

			ArrayUtility.Add(ref areaNames, "");
			ArrayUtility.Add(ref areaNames, "Open Area Settings...");

			EditorGUI.BeginProperty(position, label, property);

			EditorGUI.BeginChangeCheck();
			areaIndex = EditorGUI.Popup(position, label.text, areaIndex, areaNames);

			if (EditorGUI.EndChangeCheck())
			{
				if (areaIndex >= 0 && areaIndex < areaNames.Length - 2)
					property.intValue = GameObjectUtility.GetNavMeshAreaFromName(areaNames[areaIndex]);
				else if (areaIndex == areaNames.Length - 1)
					NavMeshEditorHelpers.OpenAreaSettings();
			}

			EditorGUI.EndProperty();
		}
	}
}
