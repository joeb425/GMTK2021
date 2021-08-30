using System.Collections.Generic;
using Attributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Data/BulletData")]
public class BulletData : ScriptableObject
{
	[SerializeField]
	public float bulletSpeed;

	[SerializeField]
	public Mesh bulletMesh;
}