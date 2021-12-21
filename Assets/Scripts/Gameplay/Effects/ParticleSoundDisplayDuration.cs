using Mantis.AttributeSystem;
using UnityEngine;

namespace DefaultNamespace.Enemy
{
	[CreateAssetMenu(menuName = "Gameplay/Effects/ParticleSoundDisplayInstant")]
	public class ParticleSoundDisplayDuration : EffectDisplay
	{
		private ParticleSystem ps;

		public override void OnApplied(EffectContext context, EffectDisplayParameters parameters)
		{
			if (parameters.audioClip)
			{
				Game.Get.GetAudioHandler().PlaySfx(parameters.audioClip);
			}

			if (parameters.particleSystem)
			{
				ps = Instantiate(parameters.particleSystem, context.attributeContainer.transform);
				ps.transform.position = context.attributeContainer.transform.position;
				ps.Play();
			}
		}

		public override void OnRemoved()
		{
			Destroy(ps);
		}
	}
}