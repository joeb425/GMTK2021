using System;
using UnityEditor.UIElements;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RadialEffect : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _lineRenderer;

	[SerializeField]
	private int numSegments = 16;

	[SerializeField]
	private float minRadius = 0.1f;

	[SerializeField]
	private float maxRadius = 1.0f;

	[SerializeField]
	private float timeToMax = 0.5f;

	private float radius;

	private float accTime;

	private void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.loop = true;
		_lineRenderer.positionCount = numSegments + 1;
		_lineRenderer.useWorldSpace = false;

		Reset();
	}

	public void Reset()
	{
		radius = minRadius;
		accTime = 0.0f;
		SetRadius(radius);
	}

	private void SetRadius(float newRadius)
	{
		radius = newRadius;
		UpdatePoints();
	}

	private void UpdatePoints()
	{
		// update line renderer
		float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0; i < numSegments + 1; i++)
		{
			float x = radius * Mathf.Cos(theta);
			float z = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, transform.position.y + 0.25f, z);
			_lineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}

	private void Update()
	{
		accTime += Time.deltaTime;
		float lerp = accTime == 0 ? 0 : accTime / timeToMax;
		Debug.Log(lerp);
		SetRadius(Mathf.Lerp(minRadius, maxRadius, lerp));

		if (lerp > 1.0f)
		{
			Destroy(gameObject);
		}
	}
}