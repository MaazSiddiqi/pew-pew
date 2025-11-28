using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0);

    [Header("Rotation Space")]
    [Tooltip("If true, the rotation will be in world space. If false, the rotation will be in local space.")]
    public bool worldSpace = false;

    // Update is called once per frame
    void Update()
    {
        if (worldSpace)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}
