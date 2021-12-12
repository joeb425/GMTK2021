using System;
using System.Linq;
using Mantis.AttributeSystem;
using UnityEngine;

namespace DefaultNamespace.Enemy
{
	[CreateAssetMenu(menuName = "Gameplay/Effects/ParticleSoundDisplayInstant")]
	public class ParticleSoundDisplayInstant : EffectDisplay
	{
		public override void OnApplied(GameplayAttributeContainer attributeContainer, EffectDisplayParameters parameters, EffectParameters effectParameters)
		{
			if (parameters.audioClip)
			{
				Game.Get.GetAudioHandler().PlaySfx(parameters.audioClip);
			}

			if (parameters.particleSystem)
			{
				ParticleSystem ps = Instantiate(parameters.particleSystem);
				ps.transform.position = attributeContainer.transform.position;
				ps.Play();
			}
		}

		public override void OnRemoved()
		{
		}
	}
}