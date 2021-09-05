using System;
using UnityEngine;
using UnityEditor;

// This component gives a GameObject a stable, non-replicatable Globally Unique IDentifier.
// It can be used to reference a specific instance of an object no matter where it is.
// This can also be used for other systems, such as Save/Load game
[Serializable, ExecuteInEditMode, DisallowMultipleComponent]
public class GuidComponent : MonoBehaviour
{
	// Unity's serialization system doesn't know about System.Guid, so we convert to a byte array
	// Fun fact, we tried using strings at first, but that allocated memory and was twice as slow
	[SerializeField]
	private byte[] serializedGuid;

	private void Reset()
	{
		CreateGuid();
	}

	// When de-serializing or creating this component, we want to either restore our serialized GUID
	// or create a new one.
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
		// Debug.Log(byteArray.Length + " " + serializedGuid.Length);
	}

	public string GetGuidString()
	{
		return serializedGuid.Length != 16 ? "null" : new System.Guid(serializedGuid).ToString();
	}

	public byte[] GetGuidBytes()
	{
		return serializedGuid;
	}

	public Guid GetGuid()
	{
		return new Guid(serializedGuid);
	}
}