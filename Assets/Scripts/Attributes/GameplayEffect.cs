using UnityEngine;

namespace Attributes
{
	[CreateAssetMenu(menuName = "Data/GameplayEffect")]
	public class GameplayEffect : ScriptableObject
	{
		[SerializeField]
		public GameplayAttributeModifier[] Modifiers;
	}
}