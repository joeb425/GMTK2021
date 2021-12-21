using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using UnityEngine;

namespace DefaultNamespace.Enemy.ModifierMagnitude
{
	[CreateAssetMenu(menuName = "Attributes/ModifierMagnitude/DamageModifierMagnitude")]
	public class DamageModifierMagnitude : ModifierMagnitide
	{
		[SerializeField]
		private float baseValue = 1.0f;

		public override float GetMagnitude(float[] modifierParameters, EffectContext context)
		{
			return modifierParameters.Length > 0 ? modifierParameters[0] : baseValue;
		}
	}
}