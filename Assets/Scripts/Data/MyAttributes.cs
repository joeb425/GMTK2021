using UnityEngine;

namespace DefaultNamespace.Data
{
	[CreateAssetMenu(menuName = "Data/MyAttributes")]
	public class MyAttributes : ScriptableObject
	{
		public static MyAttributes Get()
		{
			return GlobalData.GetAssetBindings().gameAssets.myAttributes;
		}

		[SerializeField]
		public AttributeType Health;

		[SerializeField]
		public AttributeType MaxHealth;

		[SerializeField]
		public AttributeType Range;

		[SerializeField]
		public AttributeType Damage;

		[SerializeField]
		public AttributeType AttackSpeed;

		[SerializeField]
		public AttributeType TurnSpeed;

		[SerializeField]
		public AttributeType Split;

		[SerializeField]
		public AttributeType SplashPercent;

		[SerializeField]
		public AttributeType SplashRadius;

		[SerializeField]
		public AttributeType Chain;

		[SerializeField]
		public AttributeType ChainRadius;

		[SerializeField]
		public AttributeType Speed;

		[SerializeField]
		public AttributeType Defence;
	}
}