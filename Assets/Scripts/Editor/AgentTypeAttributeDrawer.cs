using Bug.Navigation;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;

namespace Bug.Editor.Navigation
{
	[CustomPropertyDrawer(typeof(AgentTypeAttribute))]
	public class AgentTypeAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			int index = -1;
			int count = NavMesh.GetSettingsCount();
			string[] agentTypeNames = new string[count + 2];

			for (int i = 0; i < count; i++)
			{
				int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
				string name = NavMesh.GetSettingsNameFromID(id);
				agentTypeNames[i] = name;
				if (id == property.intValue)
					index = i;
			}

			agentTypeNames[count] = "";
			agentTypeNames[count + 1] = "Open Agent Settings...";

			bool validAgentType = index != -1;
			if (!validAgentType)
			{
				index = 0;
			}

			EditorGUI.BeginProperty(position, label, property);

			EditorGUI.BeginChangeCheck();
			index = EditorGUI.Popup(position, label.text, index, agentTypeNames);
			if (EditorGUI.EndChangeCheck())
			{
				if (index >= 0 && index < count)
				{
					int id = NavMesh.GetSettingsByIndex(index).agentTypeID;
					property.intValue = id;
				}
				else if (index == count + 1)
				{
					NavMeshEditorHelpers.OpenAgentSettings(-1);
				}
			}

			EditorGUI.EndProperty();
		}
	}
}
