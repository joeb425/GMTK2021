using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using ObjectPools;
using UnityEditor.Media;
using UnityEngine;

public class LaserBeamVFX : MonoBehaviour
{
	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	private Transform startVFX;

	[SerializeField]
	private Transform endVFX;

	[SerializeField]
	private List<ParticleSystem> particleSystems;

	private TargetPoint _target;

	private Transform _sourceTransform;
	private Transform _targetTransform;

	[SerializeField]
	private float duration = 0.5f;

	private void Awake()
	{
		if (lineRenderer)
		{
			lineRenderer.enabled = false;
		}
	}

	protected void UpdatePosition()
	{
		// if (_target != null && _target.IsValid())
		{
			// Vector3 startPos = lineRenderer.transform.position;
			// Vector3 endPos = _target.Position;

			Vector3 startPos = _sourceTransform.position;
			lineRenderer.SetPosition(0, startPos);

			Vector3 endPos = _targetTransform.position;
			endPos.y = startPos.y;
			lineRenderer.SetPosition(1, endPos);

			startVFX.transform.position = endPos;
			endVFX.transform.position = endPos;
		}
	}

	private void Update()
	{
		UpdatePosition();
	}

	public void SetSourceAndTarget(Transform source, Transform target)
	{
		_sourceTransform = source;
		_targetTransform = target;

		lineRenderer.enabled = true;
		UpdatePosition();
		particleSystems.ForEach(ps => ps.Play());

		// Destroy(gameObject, duration);
	}

	public void SetTarget(TargetPoint target)
	{
		_target = target;

		if (target != null)
		{
			lineRenderer.enabled = true;
			UpdatePosition();
			particleSystems.ForEach(ps => ps.Play());
		}
		else
		{
			particleSystems.ForEach(ps => ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear));
		}
	}

	protected void OnValidate()
	{
		if (lineRenderer == null)
		{
			lineRenderer = GetComponent<LineRenderer>();//.FirstOrDefault(comp => comp.name == "LaserRenderer");
		}

		if (startVFX == null)
		{
			startVFX = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "StartVFX");
		}

		if (endVFX == null)
		{
			endVFX = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "EndVFX");
		}

		// if (particleSystems.Contains())
		// {
			particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
		// }

		// if (muzzleParticle == null)
		// {
		// 	muzzleParticle = GetComponentsInChildren<ParticleSystem>().FirstOrDefault(t => t.name == "MuzzleParticle");
		// }
	}
}