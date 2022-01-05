using System;
using System.Collections.Generic;
using Mantis.AttributeSystem;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
	[SerializeField]
	public string enemyName;

	[SerializeField]
	public float health;

	[SerializeField]
	public float speed;
	
	[SerializeField]
	public float block;
}