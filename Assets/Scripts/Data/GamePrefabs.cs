using System.Collections.Generic;
using ObjectPools;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Data/GamePrefabs")]
public class GamePrefabs : ScriptableObject
{
	[SerializeField]
	public List<Tower> baseTowers;

	[SerializeField]
	public VisualTreeAsset towerBuildButton;

	[SerializeField]
	public VisualTreeAsset upgradeTowerButton;

	[SerializeField]
	public VisualTreeAsset attributeDisplayItem;

	[SerializeField]
	public BulletPool bulletPool;

	[SerializeField]
	public EnemyPool enemyPool;
}