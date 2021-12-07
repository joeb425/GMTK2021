using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mantis.AttributeSystem
{
	[Serializable]
	public class GameplayAttributeContainer
	{
		public Dictionary<AttributeType, GameplayAttribute> attributes =
			new Dictionary<AttributeType, GameplayAttribute>();

		public List<ActiveEffect> activeEffects = new List<ActiveEffect>();

		public void InitAttribute(AttributeType attributeType, float defaultValue)
		{
			if (attributes.ContainsKey(attributeType))
			{
				attributes[attributeType] = new GameplayAttribute(attributeType, defaultValue);
			}
			else
			{
				attributes.Add(attributeType,
					new GameplayAttribute(attributeType, defaultValue));
			}
		}

		public ActiveAttributeModifier ApplyModifier(GameplayAttributeModifier modifier)
		{
			GameplayAttribute attribute = attributes[modifier.attribute];
			return attribute.ApplyModifier(modifier);
		}

		public void ApplyModifierDirectly(GameplayAttributeModifier modifier)
		{
			GameplayAttribute attribute = attributes[modifier.attribute];
			attribute.ApplyModifierDirectly(modifier);
		}

		public bool RemoveModifier(ActiveAttributeModifier activeModifier)
		{
			// Debug.Log("Remove attribute " + activeModifier.modifier);
			GameplayAttribute attribute = attributes[activeModifier.modifier.attribute];
			return attribute.RemoveModifier(activeModifier);
		}

		public ActiveEffect ApplyEffect(GameplayEffect effect)
		{
			// effect has no duration, apply directly
			if (effect.duration == 0)
			{
				foreach (GameplayAttributeModifier mod in effect.Modifiers)
				{
					ApplyModifierDirectly(mod);
				}
				return null;
			}

			// effect has duration
			ActiveEffect newEffect = new ActiveEffect(effect);

			foreach (GameplayAttributeModifier mod in effect.Modifiers)
			{
				ActiveAttributeModifier activeModifier = ApplyModifier(mod);
				newEffect.activeModifiers.Add(activeModifier);
			}

			activeEffects.Add(newEffect);
			return newEffect;
		}

		public GameplayAttribute GetAttribute(AttributeType attribute)
		{
			return attributes[attribute];
		}

		public float GetCurrentValue(AttributeType attribute)
		{
			return attributes[attribute].GetValue();
		}

		public void RemoveEffect(ActiveEffect active)
		{
			// Debug.Log($"Remove effect! {active}");
			foreach (var activeModifier in active.activeModifiers)
			{
				RemoveModifier(activeModifier);
			}

			activeEffects.Remove(active);
		}

		public void Update(float deltaTime)
		{
			List<ActiveEffect> effectsToRemove = new List<ActiveEffect>();
			foreach (ActiveEffect active in activeEffects)
			{
				if (active.remainingDuration > 0)
				{
					active.remainingDuration -= deltaTime;
					if (active.remainingDuration <= 0)
					{
						effectsToRemove.Add(active);
					}
				}
			}

			foreach (ActiveEffect active in effectsToRemove)
			{
				RemoveEffect(active);
			}
		}
	}
}