using System.Collections;
using UnityEngine;

namespace Misc
{
	public class DestroyParticles : MonoBehaviour
	{
		private void Start()
		{
			Destroy(gameObject, GetComponent<ParticleSystem>().duration);
		}
	}
}