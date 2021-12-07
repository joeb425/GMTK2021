// using UnityEditor;
// using UnityEngine;
//
// namespace Mantis.GameplayTags
// {
// 	/// <summary>
// 	/// Property drawer for SerializableGuid
// 	///
// 	/// Author: Searous
// 	/// </summary>
// 	[CustomPropertyDrawer(typeof(GameplayTag))]
// 	public class GameplayTagPropertyDrawer : PropertyDrawer
// 	{
// 		private float ySep = 20;
// 		// private float buttonSize;
//
// 		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
// 		{
// 			// if (GUI.Button(new Rect(position.xMin, position.yMin, buttonSize, ySep - 2), "New"))
// 			// {
// 			// 	serializedGuid.stringValue = Guid.NewGuid().ToString();
// 			// }
// 			base.OnGUI(position, property, label);
// 			// // Start property draw
// 			// EditorGUI.BeginProperty(position, label, property);
// 			//
// 			// // Get property
// 			// SerializedProperty serializedGuid = property.FindPropertyRelative("serializedGuid");
// 			//
// 			// // Draw label
// 			// Rect size = new Rect(position.x, position.y + ySep / 2, position.width, position.height);
// 			// position = EditorGUI.PrefixLabel(size,  GUIUtility.GetControlID(FocusType.Passive), label);
// 			//
// 			// // Offsets position so we can draw the label for the field centered
// 			// position.y -= ySep / 2;
// 			//
// 			// // Update size of buttons to always fit perfeftly above the string representation field
// 			// buttonSize = position.width / 3;
// 			//
// 			// // Buttons
// 			// if (GUI.Button(new Rect(position.xMin, position.yMin, buttonSize, ySep - 2), "New"))
// 			// {
// 			// 	serializedGuid.stringValue = Guid.NewGuid().ToString();
// 			// }
// 			//
// 			// if (GUI.Button(new Rect(position.xMin + buttonSize, position.yMin, buttonSize, ySep - 2), "Copy"))
// 			// {
// 			// 	EditorGUIUtility.systemCopyBuffer = serializedGuid.stringValue;
// 			// }
// 			//
// 			// if (GUI.Button(new Rect(position.xMin + buttonSize * 2, position.yMin, buttonSize, ySep - 2), "Empty"))
// 			// {
// 			// 	serializedGuid.stringValue = "";
// 			// }
// 			//
// 			// // Draw fields - passs GUIContent.none to each so they are drawn without labels
// 			// Rect pos = new Rect(position.xMin, position.yMin + ySep, position.width, ySep - 2);
// 			// EditorGUI.PropertyField(pos, serializedGuid, GUIContent.none);
// 			//
// 			// // End property
// 			// EditorGUI.EndProperty();
// 		}
//
// 		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
// 		{
// 			// Field height never changes, so ySep * 2 will always return the proper hight of the field
// 			return ySep * 2;
// 		}
// 	}
// }