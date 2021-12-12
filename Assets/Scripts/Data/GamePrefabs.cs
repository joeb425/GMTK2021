using ObjectPools;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Data/GamePrefabs")]
public class GamePrefabs : ScriptableObject
{
	[SerializeField]
	public Tower basicTowerPrefab = default;

	[SerializeField]
	public Tower doubleTowerPrefab = default;

	[SerializeField]
	public Tower smgTowerPrefab = default;

	[SerializeField]
	public Tower sniperTowerPrefab = default;

	[SerializeField]
	public Tower rocketTowerPrefab = default;

	[SerializeField]
	public Tower supportTowerPrefab = default;

	[SerializeField]
	public Tower recoupTowerPrefab = default;

	[SerializeField]
	public Tower slowTowerPrefab = default;

	[SerializeField]
	public Tower tower9Prefab = default;

	[SerializeField]
	public Tower tower10Prefab = default;

	[SerializeField]
	public Tower tower11Prefab = default;

	[SerializeField]
	public Tower tower12Prefab = default;

	[SerializeField]
	public VisualTreeAsset towerBuildButton;

	[SerializeField]
	public VisualTreeAsset attributeDisplayItem;

	[SerializeField]
	public BulletPool bulletPool;

	[SerializeField]
	public EnemyPool enemyPool;
}