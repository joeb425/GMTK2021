using System;
using UnityEngine;

namespace Misc
{
	public class MaterialPulse : MonoBehaviour
	{
		private MeshRenderer _meshRenderer;

		public float minAlpha = 0.0f;
		public float maxAlpha = 1.0f;

		public float pulseSpeed = 3.0f;

		private void Awake()
		{
			_meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
		}

		private void Update()
		{
			if (_meshRenderer is null)
				return;

			Color newColor = _meshRenderer.material.color;
			float percent = (Mathf.Cos(Time.time * pulseSpeed) + 1.0f) * 0.5f;
			newColor.a = Mathf.Lerp(minAlpha, maxAlpha, percent); 
			_meshRenderer.material.color = newColor;
		}
	}
}