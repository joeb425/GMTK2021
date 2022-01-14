using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using UnityEngine;

namespace DefaultNamespace.Enemy
{
	[CreateAssetMenu(menuName = "AttributeSystem/ModifierLayer/EnemyDefenceLayer")]
	public class EnemyDefenceLayer : AttributeModifierLayer
	{
		public override float EditModifierValue(float value, GameplayAttributeModifier modifier)
		{
			if (modifier.attribute == MyAttributes.Get().Health)
			{
				if (modifier.valueOperator == AttributeOperator.Add)
				{
					// TODO Calc defence stats here
					return value;
				}
			}

			return value;
		}
	}
}