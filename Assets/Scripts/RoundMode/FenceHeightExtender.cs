using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FenceHeightExtender : MonoBehaviour
{
    [Tooltip("How much extra height to add to the collider (in meters).")]
    public float extraHeight = 10f;

    void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        
        // Increase the height
        Vector3 newSize = boxCollider.size;
        newSize.y += extraHeight;
        boxCollider.size = newSize;

        // Shift the center up so it extends upwards only
        Vector3 newCenter = boxCollider.center;
        newCenter.y += extraHeight * 0.5f;
        boxCollider.center = newCenter;
    }
}
