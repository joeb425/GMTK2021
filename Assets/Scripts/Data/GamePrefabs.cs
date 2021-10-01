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
	public VisualTreeAsset towerBuildButton;

	[SerializeField]
	public VisualTreeAsset attributeDisplayItem;

	[SerializeField]
	public BulletPool bulletPool;
}