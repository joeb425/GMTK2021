using System;
using UnityEngine;

[Serializable]
public class MaterialColorItem
{
	[SerializeField]
	public Material material;

	[SerializeField]
	public Color color;

	public void Apply()
	{
		material.color = color;
	}
}

[CreateAssetMenu(menuName = "Data/LevelColorPalette")]
public class LevelColorPalette : ScriptableObject
{
	[SerializeField]
	public Color cameraBackgroundColor;
	
	[SerializeField]
	public MaterialColorItem[] materialColors;

	public void Apply()
	{
		Camera.main.backgroundColor = cameraBackgroundColor;

		foreach (MaterialColorItem item in materialColors)
		{
			item.Apply();
		}
	}
}