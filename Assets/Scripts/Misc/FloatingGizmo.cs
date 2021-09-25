using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingGizmo : MonoBehaviour
{
	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(transform.position,1);

		// TODO draw hex grid

		// Vector3 pos = Camera.current.transform.position;
		// for (float y = pos.y - 800.0f; y < pos.y + 800.0f; y += height)
		// {
		// 	Gizmos.DrawLine(
		// 		new Vector3(-1000000.0f, 0.0f, Mathf.Floor(y / height) * height),
		// 		new Vector3(1000000.0f, 0.0f, Mathf.Floor(y / height) * height)); 
		// }
		//
		// for (float x = pos.x - 1200.0f; x < pos.x + 1200.0f; x += width)
		// {
		// 	Gizmos.DrawLine(
		// 		new Vector3(Mathf.Floor(x / width) * width, 0.0f, -1000000.0f),
		// 		new Vector3(Mathf.Floor(x / width) * width, 0.0f, 1000000.0f));
		// }
	}
}
