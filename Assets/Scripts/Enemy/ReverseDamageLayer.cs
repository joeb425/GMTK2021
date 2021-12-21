using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using UnityEngine;

namespace DefaultNamespace.Enemy
{
	[CreateAssetMenu(menuName = "Attributes/ModifierLayer/ReverseDamageLayer")]
	public class ReverseDamageLayer : AttributeModifierLayer
	{
		// TODO pass in attribute system
		public override float EditModifierValue(float value, GameplayAttributeModifier modifier)
		{
			if (modifier.attribute == MyAttributes.Get().Health)
			{
				if (modifier.valueOperator == AttributeOperator.Add)
				{
					if (value < 0)
					{
						return value * -0.1f;
					}
				}
			}

			return value;
		}
	}
}