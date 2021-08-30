using UnityEngine;

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

	[SerializeField]
	public Perk[] towerPerks;

	[SerializeField]
	public TowerAttributes towerAttributes;
}