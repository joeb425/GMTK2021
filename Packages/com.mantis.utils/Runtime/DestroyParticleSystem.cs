using System.Collections;
using UnityEngine;

namespace Mantis.Utils
{
	public class DestroyParticles : MonoBehaviour
	{
		private void Start()
		{
			Destroy(gameObject, GetComponent<ParticleSystem>().duration);
		}
	}
}