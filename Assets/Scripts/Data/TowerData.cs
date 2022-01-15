using System;
using System.Collections.Generic;
using Mantis.AttributeSystem;
using UnityEngine;

[Serializable]
public struct UpgradePath
{
	[SerializeField]
	public int upgradeCost;

	[SerializeField]
	public Tower tower;

	[SerializeField]
	public TowerData towerData;
}

[CreateAssetMenu(menuName = "Data/TowerData")]
public class TowerData : ScriptableObject
{
	[SerializeField]
	public string towerName;

	[SerializeField]
	public string towerDescription;

	[SerializeField]
	public Texture2D towerIcon;

	[SerializeField]
	public int towerCost = 5;

	[SerializeField]
	public int towerSell = 5;

	[SerializeField]
	public Bullet bulletPrefab;

	[SerializeField]
	public List<UpgradePath> upgradePaths;

	[SerializeField]
	public List<GameplayEffect> onHitEffects;

	[SerializeField]
	public AudioClip shootSfx;

	[SerializeField]
	public List<GameplayEffect> supportEffects = new List<GameplayEffect>();
}