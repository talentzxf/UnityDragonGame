using Fusion.Editor;
using UnityEditor;
using UnityEngine;

namespace Fusion.Addons.FSM.Editor
{
	[CustomEditor(typeof(StateMachineController), true)]
	public class StateMachineControllerEditor : UnityEditor.Editor
	{
		// Editor INTERFACE

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		public override void OnInspectorGUI()
		{
			FusionEditorGUI.InjectScriptHeaderDrawer(serializedObject);

			base.OnInspectorGUI();

			if (Application.isPlaying == false)
				return;

			var controller = target as StateMachineController;

			for (int i = 0; i < controller.StateMachines.Count; i++)
			{
				var machine = controller.StateMachines[i];

				DrawStateMachine("State Machine", machine, true);
				DrawChildMachines(machine);
			}
		}

		// PRIVATE METHODS

		private static void DrawStateMachine(string header, IStateMachine stateMachine, bool isActive)
		{
			EditorGUILayout.Space(10f);
			EditorGUILayout.LabelField($"{header}: {stateMachine.Name}", EditorStyles.boldLabel);

			var color = GUI.color;

			GUI.color = isActive == true ? Color.green : Color.gray;
			EditorGUILayout.LabelField("Active State", stateMachine.ActiveState.Name);

			GUI.color = Color.gray;
			EditorGUILayout.LabelField("Previous State", stateMachine.PreviousState.Name);

			GUI.color = color;
		}

		private static void DrawChildMachines(IStateMachine stateMachine)
		{
			EditorGUI.indentLevel++;

			for (int i = 0; i < stateMachine.States.Length; i++)
			{
				var state = stateMachine.States[i];

				for (int j = 0; j < state.ChildMachines.Length; j++)
				{
					DrawStateMachine("Sub Machine", state.ChildMachines[j], stateMachine.ActiveState == state);
				}
			}

			EditorGUI.indentLevel--;
		}
	}
}
