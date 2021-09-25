using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameAssets")]

public class GameAssets : ScriptableObject
{
	[SerializeField]
	public List<Material> spreadMaterials;

}
