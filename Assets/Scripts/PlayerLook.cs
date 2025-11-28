using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera playerCamera;
    private float xRotation = 0f;

    public float xSensitivity = 100f;
    public float ySensitivity = 100f;
    
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;

    public void ProcessLook(Vector2 input){
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime * ySensitivity);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Calculate shake offset
        float shakeOffsetX = 0f;
        float shakeOffsetY = 0f;
        if (shakeDuration > 0)
        {
            shakeOffsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            shakeOffsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            shakeDuration -= Time.deltaTime;
        }

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation + shakeOffsetX, shakeOffsetY, 0f);
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime * xSensitivity);
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
