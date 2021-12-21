using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using Mantis.GameplayTags;
using UnityEngine;

namespace DefaultNamespace.Enemy.ModifierMagnitude
{
	[CreateAssetMenu(menuName = "Attributes/ModifierMagnitude/GearModifierMagnitude")]
	public class GearModifierMagnitude : ModifierMagnitide
	{
		[SerializeField]
		private float baseValue = 1.0f;

		public override float GetMagnitude(float[] modifierParameters, EffectContext context)
		{
			GameplayTagContainer tags = context.attributeContainer.gameObject.GetComponent<GameplayTagContainer>();

			if (GameplayTagManager.instance.RequestTag("Status.Gear", out var gearTag))
			{
				int gearCount = tags.GetTagCount(gearTag);
				return gearCount * -1.0f;
			}

			return modifierParameters[0];
		}
	}
}