using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mantis.AttributeSystem
{
	// applies a list of effects to a container
	public class GameplayEffectList
	{
		private GameplayAttributeContainer _currentContainer;

		public List<GameplayEffect> effects = new List<GameplayEffect>();
		public List<ActiveEffect> activeEffects = new List<ActiveEffect>();

		public void SetContainer(GameplayAttributeContainer newContainer)
		{
			// remove all active effects
			if (_currentContainer != null)
			{
				foreach (ActiveEffect active in activeEffects)
				{
					_currentContainer.RemoveEffect(active);
				}
			}

			_currentContainer = newContainer;

			activeEffects = new List<ActiveEffect>();

			// apply all effects to new container
			if (newContainer != null)
			{
				foreach (GameplayEffect effect in effects)
				{
					TryApplyEffect(effect);
				}
			}
		}

		public bool TryApplyEffect(GameplayEffect effect)
		{
			if (_currentContainer == null) 
				return false;
			
			ActiveEffect active = _currentContainer.ApplyEffect(effect);
			activeEffects.Add(active);
			return true;
		}

		public void AddEffect(GameplayEffect effect)
		{
			effects.Add(effect);
			TryApplyEffect(effect);
		}

		public void RemoveEffect(GameplayEffect effect)
		{
			if (!effects.Contains(effect))
			{
				return;
			}

			effects.Remove(effect);

			if (_currentContainer != null)
			{
				ActiveEffect effectToRemove = activeEffects.First(active => active.effect == effect);
				_currentContainer.RemoveEffect(effectToRemove);
				activeEffects.Remove(effectToRemove);
			}
		}
	}
}