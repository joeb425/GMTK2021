using System;
using System.Collections.Generic;
using Attributes;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct UpgradeInfo
{
	[SerializeField]
	public int upgradeCost;
	
	[SerializeField]
	public GameplayEffect upgradeEffect;
}

[CreateAssetMenu(menuName = "Data/TowerData")]
public class TowerData : ScriptableObject
{
	[SerializeField]
	public string towerName;

	[SerializeField]
	public Texture2D towerIcon;

	[SerializeField]
	public int towerCost;

	[SerializeField]
	public int towerSell;

	[SerializeField, Range(0.0f, 50.0f)]
	public float attackRange = 1;

	[SerializeField, Range(0.0f, 200f)]
	public float damage = 5;

	[SerializeField, Range(0.0f, 200f)]
	public float attackSpeed = 1;

	[SerializeField, Range(0, 200)]
	public int split = 0;

	[SerializeField, Range(0.0f, 200f)]
	public float splash = 0;

	[SerializeField]
	public float bulletSpeed;

	[SerializeField]
	public Mesh bulletMesh;

	[SerializeField]
	public Material bulletMaterial;

	[SerializeField]
	public UpgradeInfo[] upgradeInfos;

	[SerializeField]
	public List<GameplayEffect> onHitEffects;

	[SerializeField]
	public AudioClip shootSfx;
}