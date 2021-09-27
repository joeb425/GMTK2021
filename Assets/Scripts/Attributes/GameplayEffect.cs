using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Attributes
{
	public class ActiveEffect
	{
		public readonly GameplayEffect effect;
		public List<ActiveAttributeModifier> activeModifiers = new List<ActiveAttributeModifier>();
		public float remainingDuration;

		public ActiveEffect(GameplayEffect effect)
		{
			this.effect = effect;
			this.remainingDuration = effect.duration;
		}
	}
	
	[CreateAssetMenu(menuName = "Data/GameplayEffect")]
	public class GameplayEffect : ScriptableObject
	{
		[SerializeField]
		public GameplayAttributeModifier[] Modifiers;

		[SerializeField]
		public float duration = -1;

		[SerializeField]
		public float interval = 0.0f;

		[SerializeField]
		private byte[] serializedGuid;

		private void Reset()
		{
			CreateGuid();
		}

		public void CreateGuid()
		{
			byte[] byteArray = Guid.NewGuid().ToByteArray();

			SerializedObject serializedObject = new SerializedObject(this);
			SerializedProperty serializedProperty = serializedObject.FindProperty("serializedGuid");
			serializedProperty.ClearArray();

			for (int i = 0; i < 16; i++)
			{
				serializedProperty.InsertArrayElementAtIndex(i);
				serializedProperty.GetArrayElementAtIndex(i).intValue = byteArray[i];
			}

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}
	}
}