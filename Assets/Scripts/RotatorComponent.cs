using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorComponent : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 90.0f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
